

namespace OnlineShoeKing.Models
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    public class About
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Column("first_image")]
        public string FirstImage { get; set; }

        [Column("updated_first_image_at")]
        public DateTime? UpdatedFirstImageAt { get; set; }

        [Column("updated_first_image_by")]
        public string UpdatedFirstImageBy { get; set; }

        [Column("first_text_header")]
        public string FirstTextHeader { get; set; }

        [Column("first_text")]
        public string FirstText { get; set; }

        [Column("updated_first_text_at")]
        public DateTime? UpdatedFirstTextAt { get; set; }

        [Column("updated_first_text_by")]
        public string UpdatedFirstTextBy { get; set; }

        [Column("second_image")]
        public string SecondImage { get; set; }

        [Column("updated_second_image_at")]
        public DateTime? UpdatedSecondImageAt { get; set; }

        [Column("updated_second_image_by")]
        public string UpdatedSecondImageBy { get; set; }

        [Column("second_text_header")]
        public string SecondTextHeader { get; set; }

        [Column("second_text")]
        public string SecondText { get; set; }

        [Column("updated_second_text_at")]
        public DateTime? UpdatedSecondTextAt { get; set; }

        [Column("updated_second_text_by")]
        public string UpdatedSecondTextBy { get; set; }
    }
}