/*
 * MSR Tools - tools for mining software repositories
 * 
 * Copyright (C) 2010  Semyon Kirnosenko
 */

using System;
using System.Linq;
using System.Data.Linq;
using System.Data.Linq.Mapping;
using NUnit.Framework;
using SharpTestsEx;

namespace MSR.Data
{
	[TestFixture]
	public class InMemoryDataStoreTest
	{
		private class Entity
		{
			[Column(DbType = "Int NOT NULL IDENTITY", IsPrimaryKey = true, IsDbGenerated = true)]
			public int ID { get; set; }
			[Column(DbType = "NVarChar(50) NOT NULL")]
			public string Data { get; set; }
			[Column(CanBeNull = true)]
			public virtual int? AnotherEntityID { get; set; }

			private EntityRef<Entity> _anotherEntity;
			[Association(ThisKey = "AnotherEntityID", OtherKey = "ID", IsForeignKey = true)]
			public Entity AnotherEntity
			{
				get { return this._anotherEntity.Entity; }
				set { this._anotherEntity.Entity = value; }
			}
		}
		
		private InMemoryDataStore data;
		
		[SetUp]
		public void SetUp()
		{
			data = new InMemoryDataStore();
		}
		[Test]
		public void Can_submit_nothing()
		{
			using (var s = data.OpenSession())
			{
				s.SubmitChanges();
			}
		}
		[Test]
		public void Should_set_id_after_submit()
		{
			using (var s = data.OpenSession())
			{
				Entity e = new Entity();
				s.Add(e);
				
				e.ID.Should().Be(0);
				
				s.SubmitChanges();
				
				e.ID.Should().Not.Be(0);
			}
		}
		[Test]
		public void Should_set_unique_id_for_each_entity()
		{
			using (var s = data.OpenSession())
			{
				Entity e1 = new Entity();
				Entity e2 = new Entity();
				Entity e3 = new Entity();

				s.Add(e1);
				s.Add(e2);
				s.Add(e3);

				s.SubmitChanges();

				s.Queryable<Entity>().Select(x => x.ID).ToArray()
					.Should().Have.UniqueValues();
			}
		}
		[Test]
		public void Should_set_foreign_id_during_submit()
		{
			using (var s = data.OpenSession())
			{
				Entity e1 = new Entity();
				Entity e2 = new Entity();

				s.Add(e1);
				s.Add(e2);
				
				e1.AnotherEntity = e2;

				s.SubmitChanges();
				
				e1.AnotherEntityID
					.Should().Be(e2.ID);
			}
		}
		[Test]
		public void Should_update_foreign_id_during_submit()
		{
			Entity e1 = new Entity();
			Entity e2 = new Entity();
			Entity e3 = new Entity();
			
			using (var s = data.OpenSession())
			{
				s.Add(e1);
				s.Add(e2);
				
				e1.AnotherEntity = e2;

				s.SubmitChanges();
			}
			using (var s = data.OpenSession())
			{
				s.Add(e3);
				
				e1.AnotherEntity = e3;
				
				s.SubmitChanges();
			}
			
			e1.AnotherEntityID
				.Should().Not.Be(e2.ID);
			e1.AnotherEntityID
				.Should().Be(e3.ID);
		}
		[Test]
		public void Should_keep_entities_between_sessions()
		{
			Entity e = new Entity();
			
			using (var s = data.OpenSession())
			{
				s.Add(e);
				s.SubmitChanges();
			}

			using (var s = data.OpenSession())
			{
				s.Queryable<Entity>().Count()
					.Should().Be(1);
				s.Queryable<Entity>().Single()
					.Should().Be.SameInstanceAs(e);
			}
		}
		[Test]
		public void Should_save_changes_after_submit()
		{
			Entity e = new Entity() { Data = "data" };

			using (var s = data.OpenSession())
			{
				s.Add(e);
				s.SubmitChanges();
			}
			using (var s = data.OpenSession())
			{
				s.Queryable<Entity>().First().Data = "";
				s.SubmitChanges();
			}
			using (var s = data.OpenSession())
			{
				s.Queryable<Entity>().First().Data
					.Should().Be("");
			}
		}
		[Test]
		[Ignore]
		public void Should_not_save_changes_before_submit()
		{
			Entity e = new Entity() { Data = "data" };

			using (var s = data.OpenSession())
			{
				s.Add(e);
				s.SubmitChanges();
			}
			using (var s = data.OpenSession())
			{
				s.Queryable<Entity>().First().Data = "";
				s.Queryable<Entity>().First().Data
					.Should().Be("");
			}
			using (var s = data.OpenSession())
			{
				s.Queryable<Entity>().First().Data
					.Should().Be("data");
			}
		}
		[Test]
		public void Should_remove_entity_after_submit()
		{
			Entity e = new Entity();

			using (var s = data.OpenSession())
			{
				s.Add(e);
				s.SubmitChanges();
			}
			using (var s = data.OpenSession())
			{
				s.Delete(e);
			}
			using (var s = data.OpenSession())
			{
				s.Queryable<Entity>().Count()
					.Should().Be(1);
			}
			using (var s = data.OpenSession())
			{
				s.Delete(e);
				s.SubmitChanges();
			}
			using (var s = data.OpenSession())
			{
				s.Queryable<Entity>().Count()
					.Should().Be(0);
			}
		}
	}
}
