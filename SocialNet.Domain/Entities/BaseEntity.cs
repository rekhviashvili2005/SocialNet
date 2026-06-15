using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocialNet.Domain.Entities
{
    public abstract class BaseEntity
    {

        //ეს კლასი გვჭირდება რო სხვა კლასებს გადავცეთ და ერთ და იგივეს წერა
        // არ მოგვიწიოს მემკვიდრეებს გავხდით
        //asbtract იმიტო გავხდადეთ რო ამ კლასს პირდაპირ ჩვენ არ გამოვიყენებთ
        //მხოლოდ სხვები დაიმემკვიდრებენ
        public Guid Id { get; set; } = Guid.NewGuid();
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
    }

}
