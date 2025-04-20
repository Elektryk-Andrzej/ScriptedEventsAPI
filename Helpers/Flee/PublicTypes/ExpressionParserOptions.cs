using System.Globalization;
using ScriptedEventsAPI.Helpers.Flee.InternalTypes;

namespace ScriptedEventsAPI.Helpers.Flee.PublicTypes;

public class ExpressionParserOptions
{
    private readonly ExpressionContext _myOwner;
    private readonly CultureInfo _myParseCulture;

    private readonly NumberStyles NumberStyles =
        NumberStyles.AllowDecimalPoint | NumberStyles.AllowExponent | NumberStyles.None;

    private PropertyDictionary _myProperties;

    internal ExpressionParserOptions(ExpressionContext owner)
    {
        _myOwner = owner;
        _myProperties = new PropertyDictionary();
        _myParseCulture = (CultureInfo)CultureInfo.InvariantCulture.Clone();
        InitializeProperties();
    }

    #region "Methods - Public"

    public void RecreateParser()
    {
        _myOwner.RecreateParser();
    }

    #endregion

    #region "Methods - Private"

    private void InitializeProperties()
    {
        DateTimeFormat = "dd/MM/yyyy";
        RequireDigitsBeforeDecimalPoint = false;
        DecimalSeparator = '.';
        FunctionArgumentSeparator = ',';
    }

    #endregion

    #region "Methods - Internal"

    internal ExpressionParserOptions Clone()
    {
        var copy = (ExpressionParserOptions)MemberwiseClone();
        copy._myProperties = _myProperties.Clone();
        return copy;
    }

    internal double ParseDouble(string image)
    {
        return double.Parse(image, NumberStyles, _myParseCulture);
    }

    internal float ParseSingle(string image)
    {
        return float.Parse(image, NumberStyles, _myParseCulture);
    }

    internal decimal ParseDecimal(string image)
    {
        return decimal.Parse(image, NumberStyles, _myParseCulture);
    }

    #endregion

    #region "Properties - Public"

    public string DateTimeFormat
    {
        get => _myProperties.GetValue<string>("DateTimeFormat");
        set => _myProperties.SetValue("DateTimeFormat", value);
    }

    public bool RequireDigitsBeforeDecimalPoint
    {
        get => _myProperties.GetValue<bool>("RequireDigitsBeforeDecimalPoint");
        set => _myProperties.SetValue("RequireDigitsBeforeDecimalPoint", value);
    }

    public char DecimalSeparator
    {
        get => _myProperties.GetValue<char>("DecimalSeparator");
        set
        {
            _myProperties.SetValue("DecimalSeparator", value);
            _myParseCulture.NumberFormat.NumberDecimalSeparator = value.ToString();
        }
    }

    public char FunctionArgumentSeparator
    {
        get => _myProperties.GetValue<char>("FunctionArgumentSeparator");
        set => _myProperties.SetValue("FunctionArgumentSeparator", value);
    }

    #endregion
}