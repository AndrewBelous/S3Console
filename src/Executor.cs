using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Amazon.S3;
using Amazon.S3.Model;

namespace S3Console
{
	internal class Executor
	{
        private AmazonS3Client _client = null;
        private string _bucketName = string.Empty;
        private int _pageSize = 10;

		public Executor()
		{
            _client = (AmazonS3Client)Amazon.AWSClientFactory.CreateAmazonS3Client(ProjConfig.S3AccessKey,
                ProjConfig.S3SecretKey, Amazon.RegionEndpoint.USEast1);
		}

		~Executor()
		{
			if (_client != null) _client.Dispose();
		}

		private bool Delete(string bucket, string key)
		{
			//get a request object
			DeleteObjectRequest request = new DeleteObjectRequest() { BucketName = bucket, Key = key };
			//execute the request and get the response
            DeleteObjectResponse response = _client.DeleteObject(request);

            return response.HttpStatusCode == System.Net.HttpStatusCode.OK;
		}

		private bool List(string bucket, string key)
		{
			ListObjectsRequest request = new ListObjectsRequest() 
                { BucketName = bucket, Prefix = key, MaxKeys = _pageSize };
            ListObjectsResponse response = _client.ListObjects(request);

            string toRemove;
            if (key.IndexOf('/') >= 0)
                toRemove = (key.EndsWith("/") ? key : key.Substring(0, key.LastIndexOf("/")));
            else
                toRemove = key;

			//print to screen
			foreach (S3Object obj in response.S3Objects)
			{
                string objPath = obj.Key.Replace(toRemove, string.Empty);
                objPath = "." + (objPath.StartsWith("/") ? string.Empty : "/") + objPath;
                    
                double sizeInKb = obj.Size / 1000d;
                //string dateAndSize = obj.LastModified.ToString() + "\t" + 
                //    sizeInKb.ToString("###,###,##0.###") + " KB";

                if (objPath.Length > Console.BufferWidth - 40)
                {
                    objPath += "\r\n";
                }

                Console.WriteLine("{0,-70}{1,-30}{2,10}", objPath, obj.LastModified.ToString(), 
                    sizeInKb.ToString("###,###,##0.###") + " KB");
			}

			return true;
		}

		private bool ListBuckets()
		{
			ListBucketsRequest request = new ListBucketsRequest();
            ListBucketsResponse response = _client.ListBuckets(request);

			foreach(S3Bucket bucket in  response.Buckets)
			{
				Console.WriteLine(bucket.BucketName + "\t" + bucket.CreationDate.ToString());
			}

			return true;
		}

		private bool GenerateDownloadUrl(string bucket, string key, int expiresMin)
		{
			var request = new GetPreSignedUrlRequest() 
            { 
                BucketName = bucket, 
                Key = key,
			    Expires = DateTime.Now.AddMinutes(expiresMin),
			    Protocol = Protocol.HTTPS,
			    Verb = HttpVerb.GET
            };

			string url = _client.GetPreSignedURL(request);

			//print to screen
			Console.WriteLine(url);
			
			return true;
		}

        private bool PageSize(int? pageSize)
        {
            if (pageSize != null)
                _pageSize = (int)pageSize;
            
            Console.WriteLine("Page size set to " + _pageSize);
            
            return true;
        }

		public bool Run(string action, string bucket = "", string key = "", 
			params string[] others)
		{
            //bucket setup
            if (String.IsNullOrWhiteSpace(bucket) && !string.IsNullOrWhiteSpace(_bucketName))
                bucket = _bucketName;

			switch (action)
			{
				case Actions.ListBuckets: return ListBuckets();
				case Actions.List: return List(bucket, key);
				case Actions.Delete: 
					if (Confirm(action, key)) return Delete(bucket, key); 
					else return true;
				case Actions.GenerateDownloadUrl:
				{
					//get minutes to expire
					int expireMin = 1440;   //1 day
					if (others.Length > 0) 
					{
                        if (!int.TryParse(others[0], out expireMin)) expireMin = 1440;
					}
					return GenerateDownloadUrl(bucket, key, expireMin);
				}
                case Actions.SetBucket:
                {
                    _bucketName = key;
                    return true;
                }
                case Actions.PageSize:
                {
                    int? pageSize = null;
                    int tmp = 0;
                    if (int.TryParse(key, out tmp))
                        pageSize = tmp;

                    return PageSize(pageSize);
                }
				default: throw new InvalidOperationException("Action " + action + " is not supported.");
			}
		}

		private bool Confirm(string action, string key)
		{
			Console.Write("Are you sure you want to " + action + " " + key + "? (y/n)");
			ConsoleKeyInfo keyPressed = Console.ReadKey(false);
			Console.WriteLine();

			return (keyPressed.Key == ConsoleKey.Y);
		}
	}	//c

    internal static class Actions
    {
        public const string ListBuckets = "list-buckets";
        public const string SetBucket = "set-bucket";
        public const string List = "list";
        public const string Delete = "delete";
        public const string GenerateDownloadUrl = "gen-download-url";
        public const string PageSize = "page-size";
    }
}	//n
