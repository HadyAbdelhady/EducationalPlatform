using Application.DTOs.Answer;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Text.Json;

namespace Infrastructure.Binders
{
    public class UpdateAnswerDtoModelBinder : IModelBinder
    {
        public async Task BindModelAsync(ModelBindingContext bindingContext)
        {
            ArgumentNullException.ThrowIfNull(bindingContext);

            var modelName = bindingContext.ModelName;
            var valueProvider = bindingContext.ValueProvider;

            // Try to get the JSON string from form data
            var valueResult = valueProvider.GetValue(modelName);
            
            if (valueResult == ValueProviderResult.None)
            {
                // No value provided - return empty list
                bindingContext.Result = ModelBindingResult.Success(new List<UpdateAnswerDto>());
                return;
            }

            var jsonValue = valueResult.FirstValue;

            if (string.IsNullOrWhiteSpace(jsonValue))
            {
                // Empty or whitespace value - return empty list
                bindingContext.Result = ModelBindingResult.Success(new List<UpdateAnswerDto>());
                return;
            }

            // Clean up the JSON string - remove any extra quotes that might be added by form data
            jsonValue = jsonValue.Trim();
            if (jsonValue.StartsWith("\"") && jsonValue.EndsWith("\""))
            {
                jsonValue = jsonValue[1..^1];
            }

            // Handle empty array case
            if (jsonValue == "[]" || jsonValue == "null")
            {
                bindingContext.Result = ModelBindingResult.Success(new List<UpdateAnswerDto>());
                return;
            }

            // Auto-wrap JSON objects in array brackets if needed
            if (!jsonValue.StartsWith("["))
            {
                jsonValue = $"[{jsonValue}]";
            }

            try
            {
                // Deserialize the JSON array
                var answers = JsonSerializer.Deserialize<List<UpdateAnswerDto>>(jsonValue, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true,
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                });

                var result = answers ?? new List<UpdateAnswerDto>();
                bindingContext.Result = ModelBindingResult.Success(result);
            }
            catch (JsonException ex)
            {
                // Provide detailed error information for debugging
                var errorMessage = $"Invalid JSON format for {modelName}. Error: {ex.Message}. Received value: '{jsonValue}'";
                bindingContext.ModelState.TryAddModelError(modelName, errorMessage);
                bindingContext.Result = ModelBindingResult.Failed();
            }
            catch (Exception ex)
            {
                // Handle any other unexpected errors
                var errorMessage = $"Unexpected error processing {modelName}: {ex.Message}. Received value: '{jsonValue}'";
                bindingContext.ModelState.TryAddModelError(modelName, errorMessage);
                bindingContext.Result = ModelBindingResult.Failed();
            }
        }
    }
}
