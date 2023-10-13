using System;
using System.Collections.Generic;
using System.Text;

namespace WorldLibrary.Prism.Models
{
    public class BookResponse
    {
        public int Id { get; set; }
               
        public string Title { get; set; }
           
        public string Author { get; set; }
             
        public string Year { get; set; }
              
        public string Synopsis { get; set; }
                
        public string Category { get; set; }
        public string Assessment { get; set; }           
               
        public double Quantity { get; set; }
              
             
        public Guid ImageId { get; set; }
        
        public UserResponse User { get; set; }

        public string ImageFullPath { get; set; }
    }
}
