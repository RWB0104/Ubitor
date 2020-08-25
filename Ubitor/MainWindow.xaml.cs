using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using Microsoft.Win32;

namespace Ubitor
{
	/// <summary>
	/// MainWindow.xaml에 대한 상호 작용 논리
	/// </summary>
	public partial class MainWindow : MetroWindow
	{
		BackgroundWorker worker = new BackgroundWorker();

		/// <summary>
		/// MainWindow 생성자 함수
		/// </summary>
		/// <returns>[void] Form 초기화</returns>
		public MainWindow()
		{
			InitializeComponent();
		}

		/// <summary>
		/// Form 로딩 후 이벤트 함수
		/// </summary>
		/// <param name="sender" type="object">이벤트 발생 객체</param>
		/// <param name="e" type="RoutedEventArgs">이벤트 객체</param>
		/// <returns>[void] 내부 함수 실행</returns>
		private void MetroWindow_Loaded(object sender, RoutedEventArgs e)
		{
			DataContext = new Method();

			Progress.Maximum = 1000000;

			worker.WorkerReportsProgress = true;
			worker.WorkerSupportsCancellation = true;
			worker.DoWork += ThreadWork;
			worker.ProgressChanged += ThreadProgress;
			worker.RunWorkerCompleted += ThreadComplete;
		}

		/// <summary>
		/// Github 버튼 클릭 이벤트 함수
		/// </summary>
		/// <param name="sender" type="object">이벤트 발생 객체</param>
		/// <param name="e" type="RoutedEventArgs">이벤트 객체</param>
		/// <returns>[void] Github 홈페이지 리다이렉트</returns>
		private void LaunchGitHub(object sender, RoutedEventArgs e)
		{
			Process.Start("https://github.com/RWB0104");
		}

		/// <summary>
		/// 전송버튼 클릭 이벤트 함수
		/// </summary>
		/// <param name="sender" type="object">이벤트 발생 객체</param>
		/// <param name="e" type="RoutedEventArgs">이벤트 객체</param>
		/// <returns>[void] 통신 요청</returns>
		private void Send_Click(object sender, RoutedEventArgs e)
		{
			worker.RunWorkerAsync();
		}

		/// <summary>
		/// 전송버튼 클릭 이벤트 함수
		/// </summary>
		/// <param name="sender" type="object">이벤트 발생 객체</param>
		/// <param name="e" type="RoutedEventArgs">이벤트 객체</param>
		/// <returns>[void] 통신 취소</returns>
		private async void Cancel_Click(object sender, RoutedEventArgs e)
		{
			var result = await this.ShowMessageAsync("Cancel", "작업을 취소하시겠습니까?", MessageDialogStyle.AffirmativeAndNegative);

			// 쓰레드 종료
			if (result == MessageDialogResult.Affirmative)
			{
				worker.CancelAsync();
			}
		}

		/// <summary>
		/// 키다운 이벤트 함수
		/// </summary>
		/// <param name="sender" type="object">이벤트 발생 객체</param>
		/// <param name="e" type="KeyEventArgs">이벤트 객체</param>
		/// <returns>[void] 전송 버튼 이벤트 연결</returns>
		private void Enter_KeyDown(object sender, KeyEventArgs e)
		{
			// Enter를 누를 경우
			if (e.Key == Key.Return)
			{
				Send_Click(sender, e);
			}
		}

		/// <summary>
		/// HTTP 통신 요청 함수
		/// </summary>
		/// <param name="sender" type="object">이벤트 발생 객체</param>
		/// <param name="e" type="RoutedEventArgs">이벤트 객체</param>
		/// <returns>[void] 통신 요청 및 응답 반환</returns>
		private void ThreadWork(object sender, DoWorkEventArgs e)
		{
			string method = null;
			string url = null;
			string referer = null;
			string agent = null;

			Dispatcher.Invoke(DispatcherPriority.Normal, new Action(delegate
			{
				Send.Visibility = Visibility.Collapsed;
				Cancel.Visibility = Visibility.Visible;

				method = Method.Text;
				url = URL.Text;
				referer = Referer.Text;
				
				// User-Agent가 비어있을 경우
				if (string.IsNullOrEmpty(Agent.Text))
				{
					agent = "Mozilla/5.0 (X11; Ubuntu; Linux x86_64; rv:35.0) Gecko/20100101 Firefox/35.0";
				}

				// User-Agent가 임의의 값을 가질 경우
				else
				{
					agent = Agent.Text;
				}
			}));
			
			Download(e, method, url, referer, agent);
		}

		/// <summary>
		/// 쓰레드 진행상황 갱신 이벤트 함수
		/// </summary>
		/// <param name="sender" type="object">이벤트 발생 객체</param>
		/// <param name="e" type="RoutedEventArgs">이벤트 객체</param>
		/// <returns>[void] 쓰레드 진행상황 프로그레스 반영</returns>
		private void ThreadProgress(object sender, ProgressChangedEventArgs e)
		{
			int value = e.ProgressPercentage;

			Progress.Value = value;
		}

		/// <summary>
		/// 쓰레드 종료 이벤트 함수
		/// </summary>
		/// <param name="sender" type="object">이벤트 발생 객체</param>
		/// <param name="e" type="RoutedEventArgs">이벤트 객체</param>
		/// <returns>[void] 종료 사유에 따른 메시지 출력</returns>
		private void ThreadComplete(object sender, RunWorkerCompletedEventArgs e)
		{
			// 오류로 인한 종료일 경우
			if (e.Error != null)
			{
				this.ShowModalMessageExternal(e.Error.GetType().FullName, e.Error.Message);
			}

			// 취소로 인한 종료일 경우
			else if (!e.Cancelled)
			{
				this.ShowModalMessageExternal("Complete", "작업이 완료되었습니다.");
			}

			InitControl();
		}

		/// <summary>
		/// 다운로드 함수 (Referer 포함)
		/// </summary>
		/// <param name="method" type="string">HTTP Method</param>
		/// <param name="url" type="string">URL 문자열</param>
		/// <param name="referer" type="string">Referer</param>
		/// <param name="agent" type="string">User Agent</param>
		/// <returns>[void] 다운로드</returns>
		private void Download(DoWorkEventArgs e, String method, String url, string referer, string agent)
		{
			bool http = url.StartsWith("http://");
			bool https = url.StartsWith("https://");
			
			// 유효한 통신 프로토콜일 경우
			if (http || https)
			{
				HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
				request.Method = method;
				request.UserAgent = agent;

				// Referer가 유효하지 않을 경우
				if (!string.IsNullOrEmpty(referer))
				{
					request.Referer = referer;
				}

				string path = GetFilePath();

				// 경로가 유효하지 않을 경우
				if (path == null)
				{
					e.Cancel = true;
				}

				// 경로가 유효할 경우
				else
				{
					HttpWebResponse response = (HttpWebResponse)request.GetResponse();

					Stream input = response.GetResponseStream();
					FileStream output = new FileStream(path, FileMode.Create);

					int length = 20480;

					byte[] buffer = new byte[length];

					double count = 0;

					while (length != 0)
					{
						// 쓰레드 취소 요청
						if (worker.CancellationPending)
						{
							e.Cancel = true;
							break;
						}

						length = input.Read(buffer, 0, buffer.Length);
						output.Write(buffer, 0, length);
						output.Flush();

						count += length;

						double percent = count / response.ContentLength * 100;

						Dispatcher.Invoke(DispatcherPriority.Normal, new Action(delegate
						{
							Percent.Content = string.Format("{0:f2}", percent) + "%";
							Value.Content = DiskValue(count) + " / " + DiskValue(response.ContentLength);
						}));

						worker.ReportProgress((int)(percent * 10000));
					}

					output.Close();
				}
			}

			// 유효하지 않은 통신 프로토콜일 경우
			else
			{
				Dispatcher.Invoke(DispatcherPriority.Normal, new Action(delegate
				{
					this.ShowModalMessageExternal("Error", "HTTP 혹은 HTTPS 프로토콜을 입력하세요.");
				}));
			}
		}

		/// <summary>
		/// 저장 경로 반환 함수
		/// </summary>
		/// <returns>[string] 저장 경로</returns>
		private string GetFilePath()
		{
			string path;

			SaveFileDialog dialog = new SaveFileDialog
			{
				Title = "저장 경로 선택",
				Filter = "Comma Separated Values text file format (ASCII) (*.csv)|*.csv|HTML Document (*.html)|*.html|MPEG audio stream, layer 3 (*.mp3)|*.mp3|Multimedia container format, MPEG 4 Part 14 (*.mp4)|*.mp4|Common name for ASCII text file (*.txt)|*.txt|All Files (*.*)|*.*",
				InitialDirectory = Environment.GetEnvironmentVariable("USERPROFILE") + @"\Downloads"
			};

			// 유효한 파일 경로를 지정할 경우
			if (dialog.ShowDialog() == true)
			{
				path = dialog.FileName;
			}

			// 파일 경로를 지정하지 않을 경우
			else
			{
				path = null;
			}

			return path;
		}

		/// <summary>
		/// 용량별 구분 및 반환 함수
		/// </summary>
		/// <param name="value" type="double">용량</param>
		/// <returns>[string] 용량별 구분 문자열</returns>
		private string DiskValue(double value)
		{
			int GB = 1024 * 1024 * 1024;
			int MB = 1024 * 1024;
			int KB = 1024;

			string result;

			// GB
			if (value >= GB)
			{
				result = string.Format("{0:f2}", value / GB) + "GB";
			}

			// MB
			else if (value >= MB)
			{
				result = string.Format("{0:f2}", value / MB) + "MB";
			}

			// KB
			else if (value >= KB)
			{
				result = string.Format("{0:f2}", value / KB) + "KB";
			}

			// BYTE
			else
			{
				result = value.ToString() + "Bytes";
			}

			return result;
		}

		/// <summary>
		/// 다운로드 관련 컨트롤 초기화 함수
		/// </summary>
		/// <returns>[void] 컨트롤 초기화</returns>
		private void InitControl()
		{
			URL.IsEnabled = true;
			Referer.IsEnabled = true;
			Agent.IsEnabled = true;

			Value.Content = "";
			Percent.Content = "";

			Progress.Value = 0;

			Send.Visibility = Visibility.Visible;
			Cancel.Visibility = Visibility.Collapsed;
		}
	}
}