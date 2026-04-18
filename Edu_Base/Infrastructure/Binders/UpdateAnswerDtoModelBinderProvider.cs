using Application.DTOs.Answer;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;

namespace Infrastructure.Binders
{
    public class UpdateAnswerDtoModelBinderProvider : IModelBinderProvider
    {
        public IModelBinder? GetBinder(ModelBinderProviderContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            // Check if the model type is List<UpdateAnswerDto>
            var modelType = context.Metadata.ModelType;
            if (modelType.IsGenericType && modelType.GetGenericTypeDefinition() == typeof(List<>))
            {
                var elementType = modelType.GetGenericArguments()[0];
                if (elementType == typeof(UpdateAnswerDto))
                {
                    return new BinderTypeModelBinder(typeof(UpdateAnswerDtoModelBinder));
                }
            }

            return null;
        }
    }
}
