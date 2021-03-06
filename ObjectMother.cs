﻿using PhotoServer.DataAccessLayer.Storage;
using PhotoServer.Domain;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;

namespace RacePhotosTestSupport
{
	public static class ObjectMother
	{
		public static string photoPath;
		public const string testDirectory = "Test.5K";

		public static void ClearDirectory(IStorageProvider provider)
		{
			// In this case, since the way we set up the test is different depending on 
			// whether we're using file storage or Azure storage, I'll switch based on what 
			// the provider is.

			if (provider is FileStorageProvider)
			{
				ClearFileStorageDirectory();
				return;
			}
			photoPath = "originals";
			var files = provider.GetFiles(photoPath);
			foreach (var file in files)
			{

				provider.DeleteFile(Path.Combine(photoPath, file));
			}

		}

		public static void ClearFileStorageDirectory()
		{
			SetPhotoPath();
			var directoryPath = Path.Combine(photoPath, "Test");
			var directoryInfo = new DirectoryInfo(photoPath);
			if (directoryInfo.Exists)
			{
				directoryInfo.Delete(true);
			}

		}

		public static void SetPhotoPath()
		{
			if (string.IsNullOrWhiteSpace(photoPath))
			{
				photoPath = ConfigurationManager.AppSettings["PhotosPhysicalDirectory"];
			}
		}

		

		private static PhotoServer.Domain.Photo[] testData =

			new PhotoServer.Domain.Photo[]
				{
					new Photo
						{
							Id = new Guid("E0CAF539-5C32-432B-AAC4-B01CD4EABB3A"),
							EventId = 1,
							Station = "FinishLine",
							Card = "1",
							Sequence = 1,
							Path = @"originals/E0CAF539-5C32-432B-AAC4-B01CD4EABB3A" ,
							Hres = 3008,
							Vres = 2000,
							TimeStamp = new DateTime(2011, 10, 22, 8, 48, 59, 60)
						},
					new Photo
						{
							Id = new Guid("24249DF5-C9FF-4B17-A259-514586F07C6B"),
							EventId = 1,
							Station = "FinishLine",
							Card = "1",
							Sequence = 3,
							Path = @"originals/24249DF5-C9FF-4B17-A259-514586F07C6B",
							Hres = 3008,
							Vres = 2000,
							TimeStamp = new DateTime(2011, 10, 22, 8, 29, 0)
						},
					new Photo
						{
							Id = new Guid("E5034D6B-3B26-4F03-8F9E-0EA1F5BE55C7"),
							EventId = 1,
							Station = "FinishLine",
							Card = "1",
							Sequence = 4,
							Path = @"originals/E5034D6B-3B26-4F03-8F9E-0EA1F5BE55C7",
							Hres = 3008,
							Vres = 2000,
							TimeStamp = new DateTime(2011, 10, 22, 8, 29, 0)
						}
				};


		public static List<PhotoServer.Domain.Photo> ReturnPhotoDataRecord(int count)
		{
			var returnList = new List<Photo>();
			if (count <= 3)
				for (int ix = 0; ix < count; ix++)
				{
					returnList.Add(testData[ix]);
				}

			return returnList;

		}

		public static void CopyTestFiles(IStorageProvider provider)
		{
			SetPhotoPath();
			var sourcePhotos = new DirectoryInfo(@"..\..\..\PhotoServer_Tests\TestFiles").EnumerateFiles().ToList();
			int ix = 0;
			foreach (var photoData in testData)
			{
				var destFile = photoData.Path;
				var sourceFile = sourcePhotos[ix++].FullName;
				using (var fileStream = new FileStream(sourceFile, FileMode.Open))
				{
					var fileData = new BinaryReader(fileStream).ReadBytes((int)fileStream.Length);
					provider.WriteFile(destFile, fileData);
				}
			}
		}
	}
}