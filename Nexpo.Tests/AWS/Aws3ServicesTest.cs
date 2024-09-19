using Moq;
using Xunit;
using Amazon.S3;
using Amazon.S3.Model;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using System.IO;
using Nexpo.AWS;

namespace Nexpo.Tests.AWS
{
    public class Aws3ServicesTest
    {
        private readonly Mock<IAmazonS3> _s3ClientMock;
        private readonly Aws3Services _aws3Services;

        public Aws3ServicesTest()
        {
            _s3ClientMock = new Mock<IAmazonS3>();

            // Provide mock AWS credentials and bucket name for testing purposes
            _aws3Services = new Aws3Services("fakeAccessKeyId", "fakeSecretAccessKey", "us-east-1", "test-bucket");
        }

        [Fact]
        [Trait("Category", "AWS")]
        public async Task UploadFileAsync_ValidFile_ReturnsTrue()
        {
            // Arrange
            var formFileMock = new Mock<IFormFile>();
            var memoryStream = new MemoryStream();
            formFileMock.Setup(f => f.CopyTo(It.IsAny<Stream>())).Callback<Stream>(s => memoryStream.CopyTo(s));
            formFileMock.Setup(f => f.FileName).Returns("test-file.txt");
            formFileMock.Setup(f => f.ContentType).Returns("text/plain");

            _s3ClientMock.Setup(x => x.PutObjectAsync(It.IsAny<PutObjectRequest>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new PutObjectResponse { HttpStatusCode = HttpStatusCode.OK });

            // Act
            var result = await _aws3Services.UploadFileAsync(formFileMock.Object, "test-file.txt");

            // Assert
            Assert.True(result);
        }

        [Fact]
        [Trait("Category", "AWS")]
        public async Task DownloadFileAsync_ValidFile_ReturnsByteArray()
        {
            // Arrange
            var memoryStream = new MemoryStream(new byte[] { 1, 2, 3 });
            var getObjectResponse = new GetObjectResponse
            {
                HttpStatusCode = HttpStatusCode.OK,
                ResponseStream = memoryStream
            };

            _s3ClientMock.Setup(x => x.GetObjectAsync(It.IsAny<GetObjectRequest>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(getObjectResponse);

            // Act
            var result = await _aws3Services.DownloadFileAsync("test-file.txt");

            // Assert
            Assert.Equal(new byte[] { 1, 2, 3 }, result);
        }

        [Fact]
        [Trait("Category", "AWS")]
        public async Task DeleteFileAsync_ValidFile_ReturnsTrue()
        {
            // Arrange
            _s3ClientMock.Setup(x => x.DeleteObjectAsync(It.IsAny<DeleteObjectRequest>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new DeleteObjectResponse { HttpStatusCode = HttpStatusCode.NoContent });

            // Act
            var result = await _aws3Services.DeleteFileAsync("test-file.txt");

            // Assert
            Assert.True(result);
        }

        [Fact]
        [Trait("Category", "AWS")]
        public void IfFileExists_ValidFile_ReturnsTrue()
        {
            // Arrange
            _s3ClientMock.Setup(x => x.GetObjectMetadataAsync(It.IsAny<GetObjectMetadataRequest>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new GetObjectMetadataResponse());

            // Act
            var result = _aws3Services.IfFileExists("test-file.txt");

            // Assert
            Assert.True(result);
        }
    }
}
