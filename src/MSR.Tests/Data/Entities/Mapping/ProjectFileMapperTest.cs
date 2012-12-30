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
using MSR.Data.Entities.Mapping.PathSelectors;

namespace MSR.Data.Entities.Mapping
{
	[TestFixture]
	public class ProjectFileMapperTest : BaseMapperTest
	{
		private ProjectFileMapper mapper;
		private List<TouchedFile> touchedFiles;
		
		[SetUp]
		public override void SetUp()
		{
			base.SetUp();
			touchedFiles = new List<TouchedFile>();
			logStub.Stub(x => x.TouchedFiles)
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
			SubmitChanges();

			Queryable<ProjectFile>().Count()
				.Should().Be(1);
			Queryable<ProjectFile>().Single()
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
			SubmitChanges();

			Queryable<ProjectFile>().Count()
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
			SubmitChanges();

			Queryable<ProjectFile>().Count()
				.Should().Be(2);
			Queryable<ProjectFile>().Single(x => x.Path == "file2")
				.Satisfy(x =>
					x.SourceFile.Path == "file1" &&
					x.SourceCommit.Revision == "9"
				);
		}
		[Test]
		public void Should_set_previous_revision_as_source_for_file_copied_without_source_revision()
		{
			mappingDSL
				.AddCommit("9").At(DateTime.Today.AddDays(-1))
					.AddFile("file1")
			.Submit();

			RenameFile("file2", "file1");
			scmData.Stub(x => x.PreviousRevision("10"))
				.Return("9");
				
			mapper.Map(
				mappingDSL.AddCommit("10")
			);
			SubmitChanges();

			Queryable<ProjectFile>().Single(x => x.Path == "file2")
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
			SubmitChanges();

			Queryable<ProjectFile>().Single().DeletedInCommit.Revision
				.Should().Be("10");
		}
		[Test]
		public void Should_use_path_selectors()
		{
			AddFile("file1.123");
			AddFile("file2.555");

			mapper.PathSelectors = new IPathSelector[] {
				new SkipPathByExtension()
				{
					Extensions = new string[] { ".555" }
				}
			};
			
			mapper.Map(
				mappingDSL.AddCommit("10")
			);
			SubmitChanges();

			Queryable<ProjectFile>()
				.Select(x => x.Path).ToArray()
					.Should().Have.SameSequenceAs(new string[] { "file1.123" });
		}
		[Test]
		public void Should_use_all_path_selectors()
		{
			AddFile("/dir1/file1.123");
			AddFile("/dir1/file2.555");
			AddFile("/dir2/file3.555");

			mapper.PathSelectors = new IPathSelector[] {
				new TakePathByList()
				{
					Dirs = new string[] { "/dir1" }
				},
				new TakePathByExtension()
				{
					Extensions = new string[] { ".555" }
				}
			};

			mapper.Map(
				mappingDSL.AddCommit("10")
			);
			SubmitChanges();

			Queryable<ProjectFile>().Count()
				.Should().Be(1);
			Queryable<ProjectFile>()
				.Single().Path
					.Should().Be("/dir1/file2.555");
		}
		[Test]
		public void Should_map_ranamed_file_during_partial_mapping()
		{
			mappingDSL
				.AddCommit("7")
					.AddFile("file1.c").Modified()
			.Submit()
				.AddCommit("8")
					.File("file1.c").Delete()
			.Submit()
				.AddCommit("9")
					.AddFile("file3.c").Modified()
			.Submit();
			
			RenameFile("file1.cpp", "file1.c");

			scmData.Stub(x => x.Log("8"))
				.Return(logStub);
			scmData.Stub(x => x.PreviousRevision("8"))
				.Return("7");
			mapper.PathSelectors = new IPathSelector[] {
				new TakePathByExtension()
				{
					Extensions = new string[] { ".cpp" }
				}
			};
			mapper.Map(
				mappingDSL.Commit("8")
			);
			SubmitChanges();

			Queryable<ProjectFile>()
				.Where(x => x.Path == "file1.cpp").Count()
					.Should().Be(1);
		}
		
		private void AddFile(string path)
		{
			TouchPath(path, TouchedFile.TouchedFileAction.ADDED, null, null);
		}
		private void ModifyFile(string path)
		{
			TouchPath(path, TouchedFile.TouchedFileAction.MODIFIED, null, null);
		}
		private void CopyFile(string path, string sourcePath, string sourceRevision)
		{
			TouchPath(path, TouchedFile.TouchedFileAction.ADDED, sourcePath, sourceRevision);
		}
		private void RenameFile(string path, string sourcePath)
		{
			DeleteFile(sourcePath);
			CopyFile(path, sourcePath, null);
		}
		private void DeleteFile(string path)
		{
			TouchPath(path, TouchedFile.TouchedFileAction.DELETED, null, null);
		}
		private void TouchPath(string path, TouchedFile.TouchedFileAction action, string sourcePath, string sourceRevision)
		{
			touchedFiles.Add(new TouchedFile()
			{
				Path = path,
				Action = action,
				SourcePath = sourcePath,
				SourceRevision = sourceRevision
			});
		}
	}
}
