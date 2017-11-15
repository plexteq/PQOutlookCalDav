using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CalDav.Outlook
{
    public class RSAKeyProvider
    {
        public static string PrivateKey {
            get {
                return "<RSAKeyValue><Modulus>4BP0Al3LDR5ywPVrFt8910uH0kgHY697fGj9LiJ0" +
                "/LKiwRSXiTOOYDlexUGiAOQ8pmnz+CxhVqg6xKwlvKP17nfKNNqKUmrUZdPJ3YmurNthZWo/m9UUAdBus5adfZQUdSMKC+2" +
                "YUxVrTfgL3ywGxvYAErlYq2iHnBSKf1jRUpILOFB3RxizbXrGpis1H65yRqms8hF+hrbokWmENNqDauFP3soMfyJUIZTLdY" +
                "/kOJM2eG+5EKT56HmJNFmnvyR1s9jc780cOyNaXjgB19muKgzwNAX8ZJ9SO9QdAxQLQkKx7LKkwekEgkts4DoVk3s0kGJ3P" +
                "rUgbvO1cmF+pGgSVQsj95RjKY/KYMGb00lF5djCQ4Z/xDk7pQBFaEQ/Xk2NaCnp0W+vzvAjIVr+qTayO+MV2sRQra9uXOMY" +
                "hlGVSanvyia5LK3PeYxUZGpUF/roVyDblnS3yK3LH+lntLmPTmPf4VlQxzjOSrQ9kUgEgSMhhiMkP97LNY8umx38qraN</M" +
                "odulus><Exponent>AQAB</Exponent><P>6eIbmGoaqLehmOLIIfHtw9/5Umtigu7vENDbdoWD7jYOOeJfLS4g7MNHljzF" +
                "E9S6oZc1ThvuY80ShK2VqTN/ytOc/3kqtUYdOZrbRO/pveC9LQ7gWyEBt0if6A8ev6y34TeCayNIuyc3MTBL9Ct3koNWbeL" +
                "pBKAvn3bPUihhocyyfgSAv1l8hkKBL08gXCieKlvdea+yXcP+MWViXgRPe1/4RU1I4vi4DaC7GEJWpzBUvhlw/TKPKGHawt" +
                "iE2YYP</P><Q>9UR6FljbmCLTo11IFnsjJz81YVXz25Pnp8M9QyhFWefdPXXsUmcIn+9Y7FSgODTmcykn89evGKATtAei6Z" +
                "66ETJ3i9Dp7vlhteq6Nw1HNYuysp6noaA/85WRjz5rKnjbPDifcZzd1Q+T7dhwjTfH3z3dHtLMxcjIoGheBGGPgxzY/Qe3B" +
                "G+8cTeS5Hvz3Hg/aesqtK+BdULNryO+QylfARzez9/pz9zt9fwiZOmuTOEbT2fLHhx6Csp9gP3HQvWj</Q><DP>kaoaR73l" +
                "pqjfw2mS/dlAnVr0XEm3TBbl3BJIbTPQrF8MUy7S2j+9j9g70K0+jRBTwGp9b41j1tKVMv8tk/kYIUnUoSRk3guyDfuNjwz" +
                "hBjpfeB2oy/jtqapYtV4dZ4F58JHy7ylFlhJhhIgw9fzQdvbHsJ6/Q/tETcvgaanuzKfBj0zcptBkBvaKoN1mKR+/CmC2uq" +
                "Abtdxoh3k+X5HNsuhdp0PwHjSL5Sjy/bPrZitqzA7qWTfRA3SWm/3gF94v</DP><DQ>jz7DHzIebqhIeu0MITUhvtZH0uVk" +
                "WyXy8iQxL3vhpTSqHi81KptKij477SsvuCQNQ80RrHSXqwYy+75KA6/UdB7Jezs0pYG484p335c9s6Q3ZeMLfHYfY6BDNyn" +
                "5ZQDY6j8XwD8hwEIs34i9k3y0m0mCT3JGbm1p32WVI3HnY2gBwyYyWcD4ihcvD4rlTipcvvu+IXEvZtDxWAzEHm2XMwsthr" +
                "L7147s2/G6FL32Mqh2TtYL5zYygQLqcd/F9PGJ</DQ><InverseQ>MEhfCPTZBj+mD4BNfFvR+m2b3b6XOprd1yDdtMzFvu" +
                "dsPqNFVtxAXmTBq/6cXL684xrIEdTPVNi0RchqBQuX66B+Fv5qZoW7118NXVyvsYxWlQwF/SFwk5yw38Lks/tNooUIbawYF" +
                "0S1aKjr6lT0IlqKdVSQ2+36lYLnQ8a5e5v06h21whrgsuufmdja8SodNZlCq7dS+BLvu0D+B22EyegMdq5DFuEC8vpDyu7K" +
                "grWKbaE7U+OPf2ahqY9202Vr</InverseQ><D>Vabq37iwc/xOivmEjMKapeAmM7f6sx0Ry7VqCad8Jle2JK3VWsmNQ7TKS" +
                "AZVgZe8ozPe4N89+dzmUeUnq5rU1+mMLnXW57gUJjQ1dmw5i7Nz4Esjs1JYinT8y8LnwmfHBht1rKeOUTvfv3bwOsdec9D2" +
                "qDGW9jZHl962LgVqRfCzPzxqy49ijpeetUfQz6Jj5iTXidQsFBx4e5Tkxzs7frBPUm7TJgmeNRPmTbLJ3uJOsOyrxfP53uY" +
                "ODyeyGjs513SqDe9uIuSYSUq4XmMrHDHPhFM5Tx62C659tPzUfWRvQ85cTiE1kils/OQNNdHxORH2cwoZHtFWkAoM4o3vqB" +
                "rKEW+54fWWUNtL5TSc0SmcuLPnyqROtlEuZC45oAPkBYpMJ3itsn8wZ/q/dO+7cg5PrdyhUfzOCztDPAlI8g9kiGQuUUUHT" +
                "wLg+LBWvu5xLJgM2j1XKqBB8xy/LnaKYlG4qCq4+LwKWlpSDH3DSk4YE294B/m3hCf5nHU3xLY1</D></RSAKeyValue>";
            }
        }
    }
}
