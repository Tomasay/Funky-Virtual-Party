using System;
using UnityEngine;

namespace Glitch9.Cloud
{
    /// <summary>
    /// Firestore와 연동할 Property앞에 붙이는 Attribute
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, Inherited = true, AllowMultiple = false)]
    public class RESTDataAttribute : PropertyAttribute
    {
        /*
            Inherited
            Inherited 매개변수는 어트리뷰트가 상속될 수 있는지를 나타냅니다. 
            이 매개변수의 값이 true일 경우, 해당 어트리뷰트가 적용된 클래스를 상속받는 하위 클래스에도 어트리뷰트가 적용됩니다. 
            즉, 상위 클래스에 정의된 어트리뷰트가 하위 클래스로 상속됩니다. 
            반면, false로 설정되면, 상위 클래스에 적용된 어트리뷰트가 하위 클래스로 자동으로 상속되지 않습니다.

            AllowMultiple
            AllowMultiple 매개변수는 동일한 요소에 대해 어트리뷰트를 여러 번 적용할 수 있는지 여부를 나타냅니다. 
            이 값이 true일 경우, 해당 어트리뷰트를 하나의 요소에 여러 번 적용할 수 있습니다. 
            예를 들어, 같은 클래스, 메서드, 혹은 프로퍼티에 동일한 어트리뷰트를 여러 개 적용할 수 있습니다. 
            false로 설정되면, 어트리뷰트를 해당 요소에 단 한 번만 적용할 수 있으며, 
            여러 번 적용하려고 시도할 경우 컴파일러 오류가 발생합니다.
            
            */

        public string PropertyName { get; private set; }

        public RESTDataAttribute(string fieldName = null)
        {
            PropertyName = fieldName;
        }
    }
}