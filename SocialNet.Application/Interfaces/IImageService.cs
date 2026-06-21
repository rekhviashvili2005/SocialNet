using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocialNet.Application.Interfaces;

public interface IImageService
{
    Task<string?> UploadImageAsync(Stream imageStream, string fileName);
}
