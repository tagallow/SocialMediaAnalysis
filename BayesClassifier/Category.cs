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
using System.Collections;
using System.Text;
using System.Text.RegularExpressions;
namespace BayesClassifier
{
	/// <summary>
	/// Represents a Enumerable Bayesian category - that is contains a list of phrases with their occurence counts <\summary>
	class EnumerableCategory : Category, IEnumerable<KeyValuePair<string, PhraseCount>>
	{
		public EnumerableCategory(string Cat, ExcludedWords Excluded) : base(Cat, Excluded)
		{
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}
		public IEnumerator<KeyValuePair<string, PhraseCount>> GetEnumerator()
		{
			return m_Phrases.GetEnumerator();
		}
 
	}


	/// <summary>
	/// Represents a Bayesian category - that is contains a list of phrases with their occurence counts <\summary>
	class Category : BayesClassifier.ICategory
	{
		protected System.Collections.Generic.SortedDictionary<string, PhraseCount> m_Phrases;
		int m_TotalWords;
		string m_Name;
		ExcludedWords m_Excluded;

		public string Name
		{
			get { return m_Name; }
			//set { m_Name = value; }
		}
		/// <value>
		/// Gets total number of word occurences in this category</value>
		public int TotalWords
		{
			get { return m_TotalWords; }
			//set { m_TotalWords = value; }
		}

		public Category(string cat, ExcludedWords excluded)
		{
			m_Phrases = new SortedDictionary<string, PhraseCount>();
			m_Excluded = excluded;
			m_Name = cat;
		}

		/// <summary>
		/// Gets a Count for Phrase or 0 if not present<\summary>
		public int GetPhraseCount(string phrase)
		{
			PhraseCount pc;
			if (m_Phrases.TryGetValue(phrase, out pc))
				return pc.Count;
			else
				return 0;
		}

		/// <summary>
		/// Reset all trained data<\summary>
		public void Reset()
		{
			m_TotalWords = 0;
			m_Phrases.Clear();
		}

		System.Collections.Generic.SortedDictionary<string, PhraseCount> Phrases
		{
			get { return m_Phrases; }
			//set { m_Phrases = value; }
		}

		/// <summary>
		/// Trains this Category from a file<\summary>
		public void TeachCategory(System.IO.TextReader reader)
		{
			//System.Diagnostics.Debug.Assert(line.Length < 512);
			Regex re = new Regex(@"(\w+)\W*", RegexOptions.Compiled);
			string line;
			while (null != (line = reader.ReadLine()))
			{
				Match m = re.Match(line);
				while (m.Success)
				{
					string word = m.Groups[1].Value;
					TeachPhrase(word);
					m = m.NextMatch();
				}
			}
		}

		/// <summary>
		/// Trains this Category from a word or phrase array<\summary>
		/// <seealso cref="DePhrase(string)">
		/// See DePhrase </seealso>
		public void TeachPhrases(string[] words)
		{
			foreach (string word in words)
			{
				TeachPhrase(word);
			}
		}

		/// <summary>
		/// Trains this Category from a word or phrase<\summary>
		/// <seealso cref="DePhrase(string)">
		/// See DePhrase </seealso>
		public void TeachPhrase(string rawPhrase)
		{
			if ((null != m_Excluded) && (m_Excluded.IsExcluded(rawPhrase)))
				return;

			PhraseCount pc;
			string Phrase = DePhrase(rawPhrase);
			if (!m_Phrases.TryGetValue(Phrase, out pc))
			{
				pc = new PhraseCount(rawPhrase);
				m_Phrases.Add(Phrase, pc);
			}
			pc.Count++;
			m_TotalWords++;
		}

		static Regex ms_PhraseRegEx = new Regex(@"\W", RegexOptions.Compiled);

		/// <summary>
		/// Checks if a string is a phrase (that is a string with whitespaces)<\summary>
		/// <returns>
		/// true or false</returns>
		/// <seealso cref="DePhrase(string)">
		/// See DePhrase </seealso>
		public static bool CheckIsPhrase(string s)
		{
			return ms_PhraseRegEx.IsMatch(s);
		}

		/// <summary>
		/// Trnasforms a string into a phrase (that is a string with whitespaces)<\summary>
		/// <returns>
		/// dephrased string</returns>
		/// <remarks>
		/// if something like "lone Rhino" is considered a sinlge Phrase, then our word matching algorithm is 
		/// is transforming it into a single Word "lone Rhino" -> "loneRhino"
		/// Currently this feature is not used!
		/// </remarks>
		public static string DePhrase(string s)
		{
			return ms_PhraseRegEx.Replace(s, @"");
		}

	}
}
