using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocialNet.Domain.Entities;

public class Follow : BaseEntity
{
    public string FollowerId { get; set; } = string.Empty;  // ვინ მიჰყვება
    public string FollowingId { get; set; } = string.Empty; // ვის მიჰყვება

}
