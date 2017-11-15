using System.IO;

namespace CalCli.API {
	public interface ISerializeToICAL {
		void Deserialize(TextReader rdr, ISerializer serializer);
		void Serialize(TextWriter wrtr);
	}
}
