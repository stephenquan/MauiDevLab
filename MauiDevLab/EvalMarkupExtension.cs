// EvalMarkupExtension.cs

using CommunityToolkit.Maui;

namespace MauiDevLab;

[ContentProperty(nameof(Expression))]
[RequireService([typeof(IReferenceProvider), typeof(IProvideValueTarget)])]
public partial class EvalExtension : BindableObject, IMarkupExtension<BindingBase>
{
	[BindableProperty(CoerceValueMethodName = nameof(CoerceExpression))]
	public partial string Expression { get; set; } = string.Empty;

	[BindableProperty]
	public partial object? Result { get; set; }

	static object CoerceExpression(BindableObject b, object v) => ((EvalExtension)b).CoerceExpression((string)v);

	object CoerceExpression(string value)
	{
		if (value is string expression
			&& !string.IsNullOrWhiteSpace(expression)
			&& new EvalParser(expression) is EvalParser parser
			&& parser.IsValid)
		{
			this.SetBinding(ResultProperty, parser.CreateBinding());
		}
		else
		{
			this.Result = null;
		}
		return value;
	}

	public object ProvideValue(IServiceProvider serviceProvider) => (this as IMarkupExtension<BindingBase>).ProvideValue(serviceProvider);
	BindingBase IMarkupExtension<BindingBase>.ProvideValue(IServiceProvider serviceProvider)
	{
		if (serviceProvider.GetService(typeof(IProvideValueTarget)) is IProvideValueTarget provideValueTarget && provideValueTarget.TargetObject is BindableObject targetObject)
		{
			this.SetBinding(BindingContextProperty, static (BindableObject b) => b.BindingContext, BindingMode.OneWay, source: targetObject);
		}
		return BindingBase.Create<EvalExtension, object?>(static e => e.Result, BindingMode.OneWay, source: this);
	}
}
