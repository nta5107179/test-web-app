using Azure.Storage.Blobs;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebApplication1
{
    public partial class SaveFile : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            string azureConnectionString = "DefaultEndpointsProtocol=https;AccountName=testblobstorage2022cxw;AccountKey=4HGkwv0YqUmpOgLjC7mChVKTTKikFjzXkbpcHCYuZk4EBJje6dX3Oh6vV8OiJIQkvhAzkXg/B052+AStux5uPg==;EndpointSuffix=core.windows.net";
            string containerName = "testcontainer";
            BlobContainerClient container = new BlobContainerClient(azureConnectionString, containerName);

            BlobClient blobClient = container.GetBlobClient("abc.txt");
            MemoryStream ms = new MemoryStream(Encoding.UTF8.GetBytes("試験ファイル"));
            // アップロード
            blobClient.Upload(ms, true);
        }
    }
}