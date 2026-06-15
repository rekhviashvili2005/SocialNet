using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocialNet.Domain.Entities;

public class Like : BaseEntity
{
    public string UserId { get; set; } = string.Empty;
    public Guid PostId { get; set; }
    public Post post { get; set; } = null!;
}
