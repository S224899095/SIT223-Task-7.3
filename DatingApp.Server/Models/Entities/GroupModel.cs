namespace DatingApp.Server.Models.Entities;

public class GroupModel
{
    public GroupModel() { }
    public GroupModel(string name)
    {
        Name = name;
    }

    [Key]
    public string Name { get; set; }
    public ICollection<ConnectionModel> Connections { get; set; } = new List<ConnectionModel>();
}
