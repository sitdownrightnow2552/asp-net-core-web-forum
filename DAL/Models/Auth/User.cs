﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using DAL.Models.Forum;
using DAL.Validation;

namespace DAL.Models.Auth
{
    public class User : Entity
    {
        [NotMapped]
        [JsonIgnore]
        public override string RawSlug => Username;

        [Column(TypeName = "NVARCHAR")]
        [StringLength(200)]
        public string Avatar { get; set; }

        [Standardized]
        [CodeId]
        [Unique]
        [Column(TypeName = "VARCHAR")]
        [MinLength(4, ErrorMessage = "Username too short!")]
        [StringLength(80, ErrorMessage = "Username too long!")]
        [Index(IsUnique = true)]
        [Required]
        public string Username { get; set; }

        [EmailAddress]
        [Unique]
        [Column(TypeName = "NVARCHAR")]
        [StringLength(200)]
        [Index(IsUnique = true)]
        [Required]
        public string Email { get; set; }

        [Column(TypeName = "VARCHAR")]
        [Password]
        [StringLength(120)]
        public string Password { get; set; }

        public virtual ICollection<Role> Roles { get; set; } = new List<Role>();

        [JsonIgnore]
        public virtual ICollection<Post> Posts { get; set; } = new List<Post>();

        [JsonIgnore]
        public virtual ICollection<Vote> Votes { get; set; } = new List<Vote>();

        [JsonIgnore]
        public UserInfo UserInfo { get; set; }

        [NotMapped]
        public ICollection<Guid> RoleIds { get; set; } = new List<Guid>();

        public string EmailConfirmToken { get; set; } = string.Empty;
    }
}