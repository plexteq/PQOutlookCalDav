namespace CalCli.API
{
	public interface IHasParameters {
		XNameValueCollection GetParameters();
		void Deserialize(string value, XNameValueCollection parameters);
	}
}
