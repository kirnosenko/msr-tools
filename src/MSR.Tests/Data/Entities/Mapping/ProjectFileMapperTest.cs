/*
 * MSR Tools - tools for mining software repositories
 * 
 * Copyright (C) 2010-2011  Semyon Kirnosenko
 */

using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using SharpTestsEx;
using Rhino.Mocks;

using MSR.Data.VersionControl;
using MSR.Data.Entities.DSL.Mapping;

namespace MSR.Data.Entities.Mapping
{
	[TestFixture]
	public class ProjectFileMapperTest : BaseMapperTest
	{
		private ProjectFileMapper mapper;
		
		private List<TouchedPath> touchedFiles;
		
		[SetUp]
		public override void SetUp()
		{
			base.SetUp();
			touchedFiles = new List<TouchedPath>();
			logStub.Stub(x => x.TouchedPaths)
				.Return(touchedFiles);
			scmData.Stub(x => x.Log("10"))
				.Return(logStub);
			
			mapper = new ProjectFileMapper(scmData);
		}
		[Test]
		public void Should_map_added_file()
		{
			AddFile("file1");
			
			mapper.Map(
				mappingDSL.AddCommit("10")
			);
			Submit();
			
			Repository<ProjectFile>().Count()
				.Should().Be(1);
			Repository<ProjectFile>().Single()
				.Satisfy(f =>
					f.Path == "file1" &&
					f.AddedInCommit.Revision == "10"
				);
		}
		[Test]
		public void Should_not_map_anything_for_modified_file()
		{
			mappingDSL
				.AddCommit("9").At(DateTime.Today.AddDays(-1))
					.AddFile("file1")
			.Submit();

			ModifyFile("file1");

			ProjectFile file = 
			mapper.Map(
				mappingDSL.AddCommit("10")
			).Single().CurrentEntity<ProjectFile>();
			Submit();
			
			Repository<ProjectFile>().Count()
				.Should().Be(1);
			file.AddedInCommit.Revision
				.Should().Be("9");
		}
		[Test]
		public void Should_map_copied_file_with_source()
		{
			mappingDSL
				.AddCommit("9").At(DateTime.Today.AddDays(-1))
					.AddFile("file1")
			.Submit();

			CopyFile("file2", "file1", "9");
			
			mapper.Map(
				mappingDSL.AddCommit("10")
			);
			Submit();

			Repository<ProjectFile>().Count()
				.Should().Be(2);
			Repository<ProjectFile>().Single(x => x.Path == "file2")
				.Satisfy(x =>
					x.SourceFile.Path == "file1" &&
					x.SourceCommit.Revision == "9"
				);
		}
		[Test]
		public void Should_set_last_revision_as_source_for_copied_file_without_source_revision()
		{
			mappingDSL
				.AddCommit("9").At(DateTime.Today.AddDays(-1))
					.AddFile("file1")
			.Submit();

			RenameFile("file2", "file1");

			mapper.Map(
				mappingDSL.AddCommit("10")
			);
			Submit();

			Repository<ProjectFile>().Single(x => x.Path == "file2")
				.Satisfy(x =>
					x.SourceFile.Path == "file1" &&
					x.SourceCommit.Revision == "9"
				);
		}
		[Test]
		public void Should_map_deleted_file()
		{
			mappingDSL
				.AddCommit("9").At(DateTime.Today.AddDays(-1))
					.AddFile("file1")
			.Submit();

			DeleteFile("file1");

			mapper.Map(
				mappingDSL.AddCommit("10")
			);
			Submit();

			Repository<ProjectFile>().Single().DeletedInCommit.Revision
				.Should().Be("10");
		}
		[Test]
		public void Can_ignore_files_by_extension()
		{
			AddFile("file1.123");
			AddFile("file2.555");

			mapper = new ProjectFileMapper(
				scmData,
				new IPathSelector[] {
					new SkipPathByExtension(new string[]
					{
						".555"
					})
				}
			);
			
			mapper.Map(
				mappingDSL.AddCommit("10")
			);
			Submit();
			
			Repository<ProjectFile>()
				.Select(x => x.Path).ToArray()
					.Should().Have.SameSequenceAs(new string[] { "file1.123" });
		}
		
		private void AddFile(string path)
		{
			TouchPath(path, TouchedPath.TouchedPathAction.ADDED, null, null);
		}
		private void ModifyFile(string path)
		{
			TouchPath(path, TouchedPath.TouchedPathAction.MODIFIED, null, null);
		}
		private void CopyFile(string path, string sourcePath, string sourceRevision)
		{
			TouchPath(path, TouchedPath.TouchedPathAction.ADDED, sourcePath, sourceRevision);
		}
		private void RenameFile(string path, string sourcePath)
		{
			DeleteFile(sourcePath);
			CopyFile(path, sourcePath, null);
		}
		private void DeleteFile(string path)
		{
			TouchPath(path, TouchedPath.TouchedPathAction.DELETED, null, null);
		}
		private void TouchPath(string path, TouchedPath.TouchedPathAction action, string sourcePath, string sourceRevision)
		{
			touchedFiles.Add(new TouchedPath()
			{
				Path = path,
				Action = action,
				SourcePath = sourcePath,
				SourceRevision = sourceRevision
			});
		}
	}
}
