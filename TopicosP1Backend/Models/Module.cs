using System.ComponentModel.DataAnnotations;

namespace CareerApi.Models
{
    public class Module
    {
        [Key]
        public long Number { get; set; }
        public IEnumerable<Room> Rooms { get; set; } = new List<Room>();
        public ModuleDTO Simple() => new(this);

        public class ModuleDTO(Module module)
        {
            public long Number = module.Number;
            public IEnumerable<Room.RoomDTO> Rooms { get; } =
                from a in module.Rooms select a.Simple();
        }
    }
}
