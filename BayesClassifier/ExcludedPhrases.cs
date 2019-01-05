/* ====================================================================
 * Copyright (c) 2006 Erich Guenther (erich_guenther@hotmail.com)
 * All rights reserved.
 *                       
 * Redistribution and use in source and binary forms, with or without
 * modification, are permitted provided that the following conditions
 * are met:
 *
 * 1. Redistributions of source code must retain the above copyright
 *    notice, this list of conditions and the following disclaimer. 
 *
 * 2. Redistributions in binary form must reproduce the above copyright
 *    notice, this list of conditions and the following disclaimer in
 *    the documentation and/or other materials provided with the
 *    distribution.
 * 
 * 3. The name of the author(s) must not be used to endorse or promote 
 *    products derived from this software without prior written permission.
 *
 * THIS SOFTWARE IS PROVIDED BY THE AUTHOR ``AS IS'' AND ANY
 * EXPRESSED OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE
 * IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR
 * PURPOSE ARE DISCLAIMED.  IN NO EVENT SHALL THE AUTHOR OR
 * ITS CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL,
 * SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT
 * NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES;
 * LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION)
 * HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT,
 * STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE)
 * ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED
 * OF THE POSSIBILITY OF SUCH DAMAGE. 
 */
using System;
using System.Collections.Generic;
using System.Text;

namespace BayesClassifier
{
	/// <summary>
	/// serves to exclude certain words from the bayesian classification</summary>
	class ExcludedWords  
	{
		/// <summary>
		/// List of english words i'm not interested in</summary>
		/// <remarks>
		/// You might use frequently used words for this list
		/// </remarks>
		static string[] enu_most_common =
		{
			 "the", 
			 "to", 
			 "and", 
			 "a", 
			 "an", 
			 "in", 
			 "is", 
			 "it", 
			 "you", 
			 "that", 
			 "was", 
			 "for", 
			 "on", 
			 "are", 
			 "with", 
			 "as", 
			 "be", 
			 "been", 
			 "at", 
			 "one", 
			 "have", 
			 "this", 
			 "what", 
			 "which", 
		};

		Dictionary<string, int> m_Dict;

		public ExcludedWords()
		{
			m_Dict = new Dictionary<string, int>();
		}

		/// <summary>
		/// Initializes for english</summary>
		public void InitDefault()
		{
			Init(enu_most_common);
		}
		public void Init(string[] excluded)
		{
			m_Dict.Clear();
			for (int i = 0; i < excluded.Length; i++)
			{
				m_Dict.Add(excluded[i], i);
			}
		}
		/// <summary>
		/// checks to see if a word is to be excluded</summary>
		public bool IsExcluded(string word)
		{
			return m_Dict.ContainsKey(word);
		}

	}
}
