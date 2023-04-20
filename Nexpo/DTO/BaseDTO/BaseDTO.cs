namespace Nexpo.DTO
{
    public abstract class BaseDTO<DTOType, EntryType>
    {

        // Apply the DTOType to all entries
        public virtual EntryType ApplyToEntries(EntryType entries)
        {
            var entryAttributes = entries.GetType().GetProperties();
            var dtoAttributes = this.GetType().GetProperties();

            foreach (var entryAttribute in entryAttributes)
            {
                foreach (var dtoAttribute in dtoAttributes)
                {
                    if (entryAttribute.Name == dtoAttribute.Name)
                    {
                        entryAttribute.SetValue(entries, dtoAttribute.GetValue(this));
                    }
                }
            }
            
            return entries;
        }
    } 
}
