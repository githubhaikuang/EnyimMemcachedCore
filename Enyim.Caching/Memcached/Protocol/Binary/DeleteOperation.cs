using System.Collections.Generic;

namespace Enyim.Caching.Memcached.Protocol.Binary
{
	public class DeleteOperation : BinarySingleItemOperation, IDeleteOperation
	{
		public DeleteOperation(string key) : base(key) { }

		protected override BinaryRequest Build()
		{
			var request = new BinaryRequest(OpCode.Delete)
			{
				Key = this.Key
			};

			return request;
		}

		protected internal override bool ReadResponse(PooledSocket socket)
		{
			var response = new BinaryResponse();

			return response.Read(socket);
		}
	}
}

#region [ License information          ]
/* ************************************************************
 * 
 *    Copyright (c) 2010 Attila Kisk�, enyim.com
 *    
 *    Licensed under the Apache License, Version 2.0 (the "License");
 *    you may not use this file except in compliance with the License.
 *    You may obtain a copy of the License at
 *    
 *        http://www.apache.org/licenses/LICENSE-2.0
 *    
 *    Unless required by applicable law or agreed to in writing, software
 *    distributed under the License is distributed on an "AS IS" BASIS,
 *    WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 *    See the License for the specific language governing permissions and
 *    limitations under the License.
 *    
 * ************************************************************/
#endregion
