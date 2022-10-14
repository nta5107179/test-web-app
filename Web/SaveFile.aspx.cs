using Azure.Storage.Blobs;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Web
{
    public partial class SaveFile : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            string azureConnectionString = "DefaultEndpointsProtocol=https;AccountName=testblobstorage2022cxw2;AccountKey=AAIg667FOr0n8umYvQL+pCjo8uUe+6s+gBOfBunb4ywQUkd+7MM1c3vrBiCF2iFX/0w9krHEo845+ASt8IjO9A==;EndpointSuffix=core.windows.net";
            string containerName = "testcontainer";
            BlobContainerClient container = new BlobContainerClient(azureConnectionString, containerName);

            // アップロードするファイルパス
            string fileName = Path.GetFileName("abc.txt");
            BlobClient blobClient = container.GetBlobClient(fileName);

            // アップロード
            using (MemoryStream ms = new MemoryStream(Encoding.UTF8.GetBytes("試験ファイル")))
            {
                blobClient.Upload(ms, true);
            };
        }
    }
}