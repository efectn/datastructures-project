using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace datastructures_project.Database.Model;

[Table("documents")]
public class Document
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    private string _title;
    private string _url;
    private string _description;

    public Document() { }

    public Document(string title, string url, string description)
    {
        Title = title;
        Url = url;
        Description = description;
    }

    public string Title
    {
        get => _title;
        set
        {
            if (string.IsNullOrEmpty(value))
            {
                throw new ArgumentException("Title cannot be empty");
            }

            if (value.Length > 50)
            {
                throw new ArgumentException("Title cannot exceed 50 characters");
            }

            _title = value;
        }
    }

    public string Url
    {
        get => _url;
        set
        {
            if (string.IsNullOrEmpty(value))
            {
                throw new ArgumentException("URL cannot be empty");
            }

            if (!IsUrlValid(value))
            {
                throw new ArgumentException("URL is not a valid URL");
            }

            _url = value;
        }
    }

    public string Description
    {
        get => _description;
        set
        {
            if (string.IsNullOrEmpty(value))
            {
                _description = "";
            }
            else
            {
                _description = value.Length > 200 ? value.Substring(0, 200) : value;
            }
        }
    }

    private static bool IsUrlValid(string url)
    {
        return Uri.IsWellFormedUriString(url, UriKind.Absolute);
    }
}