namespace Nexpo.DTO
{
    public abstract class BaseDTO<EntryType>
    {
        // CURRENTLY UNUSED

        /// <summary>
        /// Applies the values of this DTO to the given EntryType
        /// </summary>
        /// <param name="entries">The EntryType to apply the values to.
        ///                       Refers to Model entries, such as "Contact"
        ///                       That match the DTO</param>
        /// <remarks> Note that the method only works if the DTO and the EntryType
        ///           have the same property names. </remarks>
        public virtual EntryType ApplyToEntries(EntryType entries)
        {
            //Lös null check. Gör så det bara fungerar på primitiva typer. 
            // Lös med required attribut.
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
