namespace demo { 

public interface IPasswordValidator
    {
        bool Validate(string password);
        string ErrorMessage { get; }
    }
    public class MinLengthValidator : IPasswordValidator
    {
        private readonly int _minLength;

        public MinLengthValidator(int minLength)
        {
            _minLength = minLength;
        }

        public bool Validate(string password)
        {
            return password.Length >= _minLength;
        }

        public string ErrorMessage => $"Password must be at least {_minLength} characters long.";
    }
    public class DigitValidator : IPasswordValidator
    {
        public bool Validate(string password)
        {
            return password.Any(char.IsDigit);
        }

        public string ErrorMessage => "Password must contain at least one digit.";
    }
    public class SpecialCharacterValidator : IPasswordValidator
    {
        public bool Validate(string password)
        {
            return password.Any(ch => !char.IsLetterOrDigit(ch));
        }

        public string ErrorMessage => "Password must contain at least one special character.";
    }


    public class CompositePasswordValidator : IPasswordValidator
    {
        private readonly List<IPasswordValidator> _validators;

        public CompositePasswordValidator(IEnumerable<IPasswordValidator> validators)
        {
            _validators = new List<IPasswordValidator>(validators);
        }

        public bool Validate(string password)
        {
            return _validators.All(validator => validator.Validate(password));
        }

        public string ErrorMessage
        {
            get
            {
                var failedValidators = _validators.Where(validator => !validator.Validate(string.Empty));
                return string.Join(Environment.NewLine, failedValidators.Select(v => v.ErrorMessage));
            }
        }
    }

    class Program
    {
        static void Main()
        {
            // Create individual validators
            IPasswordValidator minLengthValidator = new MinLengthValidator(8);
            IPasswordValidator digitValidator = new DigitValidator();
            IPasswordValidator specialCharacterValidator = new SpecialCharacterValidator();

            // Combine validators into a composite validator
            IPasswordValidator compositeValidator = new CompositePasswordValidator(new IPasswordValidator[]
            {
            minLengthValidator,
            digitValidator,
            specialCharacterValidator
            });

            // Take user input
            Console.WriteLine("Enter a password:");
            string password = Console.ReadLine();

            // Validate the password
            if (compositeValidator.Validate(password))
            {
                Console.WriteLine("Password is valid.");
            }
            else
            {
                Console.WriteLine("Password is invalid.");
                foreach (var validator in new List<IPasswordValidator> { minLengthValidator, digitValidator, specialCharacterValidator })
                {
                    if (!validator.Validate(password))
                    {
                        Console.WriteLine(validator.ErrorMessage);
                    }
                }
            }
        }
    }
}


