using System.Collections.Generic;
using UnityEngine;

namespace Masamune.Sample.Localize {
   /// <summary>
   /// Class LocalizeScriptableObject.
   /// Implements the <see cref="UnityEngine.ScriptableObject" />
   /// </summary>
   /// <seealso cref="UnityEngine.ScriptableObject" />
   [CreateAssetMenu( menuName = "Masamune Sample/Localize/Localize Sample", fileName = "LocalizeSample", order = -500 )]
   public class LocalizeScriptableObject : ScriptableObject {

      /// <summary>
      /// The key
      /// </summary>
      [SerializeField]
      public string text;

   }
}
