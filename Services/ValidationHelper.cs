namespace andecr.Services;

public class ValidationHelper
{
   /// <summary>
   /// Checks whether all required fields contain data.
   /// </summary>
   /// <param name="fields">Collection of (field name, field value) pairs.</param>
   /// <returns>True if every field has non-empty, non-whitespace data.</returns>
   public static bool AreRequiredFieldsFilled(IEnumerable<(string Name, string? Value)> fields)
      => fields.All(f => !string.IsNullOrWhiteSpace(f.Value));

   /// <summary>
   /// Same check, but also returns the names of empty required fields —
   /// useful for highlighting them in the UI or building an error message.
   /// </summary>
   public static (bool IsValid, IReadOnlyList<string> MissingFields) ValidateRequiredFields(
      IEnumerable<(string Name, string? Value)> fields)
   {
      var missing = fields
         .Where(f => string.IsNullOrWhiteSpace(f.Value))
         .Select(f => f.Name)
         .ToList();

      return (missing.Count == 0, missing);
   }
}