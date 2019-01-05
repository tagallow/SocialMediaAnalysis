using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Xml;

//http://www.codeproject.com/Articles/14270/A-Naive-Bayesian-Classifier-in-C

namespace SocialMonitorCloud
{
	public class SentimentClassificationService
	{
		private BayesClassifier.Classifier m_Classifier = new BayesClassifier.Classifier();
		private static SentimentClassificationService _Instance;
		
		public SentimentClassificationService()
		{
			//Train();
		}
		public static SentimentClassificationService Instance
		{
			get
			{
				if (_Instance == null)
				{
					_Instance = new SentimentClassificationService();
					_Instance.Train();
				}
				return _Instance;
			}
		}
		/**
		 * The key/value closest to zero (largest) is the classification
		 * Returns true for positive and false for negative.
		 */
		public string Classify(string str)
		{
            string classification = "Error";
            try
            {
                
                byte[] byteArray = Encoding.ASCII.GetBytes(str);
                using (MemoryStream m = new MemoryStream(byteArray))
                {
                    using (System.IO.StreamReader reader = new System.IO.StreamReader(m))
                    {
                        Dictionary<string, double> score = m_Classifier.Classify(reader);
                        if (score["Positive"] >= score["Negative"])
                            classification = "positive";
                        else
                            classification = "negative";
                        System.Diagnostics.Debug.WriteLine(str + ": " + classification);
                    }
                }
                
            }
            catch(Exception e)
            {
                Console.WriteLine("Error with classification: {0}",e.Message);

            }
            return classification;
			
		}
		public void Train()
		{
			TrainFromDBPositive();
			TrainFromDBNegative();
            Console.WriteLine("Sentiment Analyzer Trained");
		}
		
		/**
		 * Learning pre loaded knowledge base
		 */
		public void TrainFromDBPositive()
		{
			List<string> PositiveWords = new List<string>();
			//List<string> NegativeWords = new List<string>();
            using (SqlConnection connection = new SqlConnection(ServerConfiguration.GetDictionaryConnectionString()))
			{
				connection.Open();
				using (SqlCommand command = connection.CreateCommand())
				{
					command.CommandText = "SELECT PositiveWords from PositiveTrainingData";
					//command.CommandType = CommandType.StoredProcedure;
					//command.Parameters.AddWithValue("@UserName", name);
					using (SqlDataReader reader = command.ExecuteReader())
					{
						while (reader.Read())
						{
							PositiveWords.Add(reader["PositiveWords"].ToString());
							//NegativeWords.Add(reader["Negative"].ToString());
						}
					}
				}
				connection.Close();
			}
			m_Classifier.TeachPhrases("Positive", PositiveWords.ToArray());
			//m_Classifier.TeachPhrases("Negative", NegativeWords.ToArray());
		}
		public void TrainFromDBNegative()
		{
			//List<string> PositiveWords = new List<string>();
			List<string> NegativeWords = new List<string>();
            using (SqlConnection connection = new SqlConnection(ServerConfiguration.GetDictionaryConnectionString()))
			{
				connection.Open();
				using (SqlCommand command = connection.CreateCommand())
				{
					command.CommandText = "SELECT NegativeWords from NegativeTrainingData";
					//command.CommandType = CommandType.StoredProcedure;
					//command.Parameters.AddWithValue("@UserName", name);
					using (SqlDataReader reader = command.ExecuteReader())
					{
						while (reader.Read())
						{
							//PositiveWords.Add(reader["NegativeWords"].ToString());
							NegativeWords.Add(reader["NegativeWords"].ToString());
						}
					}
				}
				connection.Close();
			}
			//m_Classifier.TeachPhrases("Positive", PositiveWords.ToArray());
			m_Classifier.TeachPhrases("Negative", NegativeWords.ToArray());
		}
		/**
		 * For manually training the knowledge base
		 */
		public void Train(string pos, string neg)
		{
            using (SqlConnection connection = new SqlConnection(ServerConfiguration.GetDictionaryConnectionString()))
			{
				connection.Open();
				using (SqlCommand command = connection.CreateCommand())
				{
					command.CommandText = "insert into SentimentAnalysisTrainingData (Positive, Negative) values (@pos,@neg)";
					//command.CommandType = CommandType.StoredProcedure;
					command.Parameters.AddWithValue("@pos", pos);
					command.Parameters.AddWithValue("@neg", neg);

					command.ExecuteNonQuery();
					
				}
				connection.Close();
			}
		List<string> str = new List<string>();
		str.Add(pos);
		m_Classifier.TeachPhrases("Positive", str.ToArray());
		str.Clear();
		str.Add(neg);
		m_Classifier.TeachPhrases("Negative",str.ToArray());
		}
		public string getViralHeat(string phrase)
		{
			string str = "";
			return str;
		}
	}
}