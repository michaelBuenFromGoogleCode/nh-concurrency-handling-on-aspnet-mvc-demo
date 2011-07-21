using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace NhConcurrencyOnAspNetMvc.Models
{
    public class Song : IValidatableObject
    {
        public virtual int SongId { get; set; }
        [Required] public virtual string SongName { get; set; }
        [Required] public virtual string AlbumName { get; set; }

        public virtual byte[] Version { get; set; }

        public virtual IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {

            // model-level validation
            if (SongName == "blah" && AlbumName == "meh")
            {
                yield return new ValidationResult("Combined song name and album name cannot be blah and meh");
                yield return new ValidationResult("*", new[] { "SongName", "AlbumName" });                
            }
            else
            {

                // model-level validation
                if (SongName[0] != AlbumName[0])
                {
                    yield return new ValidationResult("Song name and album name must start with same letter");
                    yield return new ValidationResult("*", new[] { "AlbumName", "SongName" });                    
                }

                // inline style
                if (!SongName.ToUpper().Contains("LOVE"))
                    yield return new ValidationResult("Song name must have a word love", new[] { "SongName" });

                // property, but asterisk style
                if (!AlbumName.ToUpper().Contains("GREAT"))
                {
                    yield return new ValidationResult("Album name must have a word great");
                    yield return new ValidationResult("*", new[] { "AlbumName" });                    
                }
            }
            
        }
    }
}