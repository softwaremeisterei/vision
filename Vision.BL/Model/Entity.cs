using System;

namespace Vision.BL.Model
{
    [Serializable]
    public abstract class Entity
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime DateModified { get; set; }
        public int OrderIndex { get; set; }

        public Entity()
        {
            Id = Guid.NewGuid();
            Name = "_Noname_";
            DateCreated = DateTime.Now;
        }
    }
}