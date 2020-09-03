using System.Collections.Generic;

namespace Ubitor
{
	/// <summary>
	/// HTTP Method 관리 클래스
	/// </summary>
	class Method
	{
		/// <summary>
		/// MethodList 리스트
		/// </summary>
		/// <returns>[IList<string>] MethodList Getter Setter</returns>
		public IList<string> MethodList
		{
			get;
			set;
		}

		/// <summary>
		/// Method 생성자 함수
		/// </summary>
		/// <returns>[void] MethodList 초기화</returns>
		public Method()
		{
			MethodList = new List<string>
			{
				"GET",
				"POST",
				"PUT",
				"PATCH",
				"DELETE",
				"COPY",
				"HEAD",
				"OPTIONS",
				"LINK",
				"UNLINK",
				"PURGE",
				"LOCK",
				"UNLOCK",
				"PROPFIND",
				"VIEW"
			};
		}
	}
}
