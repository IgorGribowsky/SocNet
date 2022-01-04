using System;
using System.ComponentModel.DataAnnotations;

namespace SocNet.Core.Entities;

public class Post
{
    public int Id { get; init; }

    [Required]
    public string Content { get; set; }

    [Required]
    public int UserId { get; set; }

    public int? ParentPostId { get; set; }

    public DateTime CreationTime { get; set; }

    public int LikeCount { get; set; }

    public int CommentCount { get; set; }
}
