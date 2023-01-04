namespace CompressionLibrary.Validators
{
    internal struct ImageCompressorValidator
    {
        public ValidatorResponse ValidationReponse { get; private set; } = new();

        public ImageCompressorValidator()
        {

        }

        internal Task LogResponse(ValidatorResponse response)
        {
            switch (response.Response)
            {
                case ResponseType.InvalidImageExtension:
                    //await _logger.ErrorAsync("Image extension is invalid.");
                    break;
                case ResponseType.FileDoesNotExist:
                    //await _logger.ErrorAsync("One or more files does not exist.");
                    break;
                case ResponseType.ListEmptyOrNull:
                    //await _logger.ErrorAsync("Provided list is empty and/or null");
                    break;
                case ResponseType.Valid:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            return Task.CompletedTask;
        }

        internal async Task<bool> AreImagePathsValidAsync(List<string> imagePaths)
        {
            if (imagePaths.Any(string.IsNullOrEmpty))
            {
                ValidationReponse.SetResponse(ResponseType.ListEmptyOrNull);
                return false;
            }

            var isValid = true;
            foreach (var path in imagePaths)
            {
                if (!File.Exists(path))
                {
                    ValidationReponse.SetResponse(ResponseType.FileDoesNotExist);
                    return !isValid;
                }
                (isValid, ValidationReponse) = await IsImageExtensionValid(path);
            }

            ValidationReponse.SetResponse(ResponseType.Valid);
            return isValid;
        }

        private static Task<(bool, ValidatorResponse)> IsImageExtensionValid(string imagePath)
        {
            var fileExtension = Path.GetExtension(imagePath);
            if (fileExtension != ".png" || fileExtension  != ".jpg")
            {
                return Task.FromResult((false, new ValidatorResponse(ResponseType.InvalidImageExtension)));
            }

            return Task.FromResult((true, new ValidatorResponse(ResponseType.Valid)));
        }





    }
}
