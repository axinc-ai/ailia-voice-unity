/* ailia Voice Unity Plugin Native Interface */
/* Copyright 2023 AXELL CORPORATION */

using UnityEngine;
using System.Collections;
using System.IO;
using System;
using System.Runtime.InteropServices;

using ailia;
using ailiaAudio;

namespace ailiaVoice{
public class AiliaVoice
{
	/* Native Binary 定義 */

	#if (UNITY_IPHONE && !UNITY_EDITOR) || (UNITY_WEBGL && !UNITY_EDITOR)
		public const String LIBRARY_NAME="__Internal";
	#else
		#if (UNITY_ANDROID && !UNITY_EDITOR) || (UNITY_STANDALONE_OSX || UNITY_EDITOR_OSX)
			public const String LIBRARY_NAME="ailia_voice";
		#else
			public const String LIBRARY_NAME="ailia_voice";
		#endif
	#endif

	/****************************************************************
	* アルゴリズム定義
	**/

	/**
	* \~japanese
	* @def AILIA_VOICE_DICTIONARY_TYPE_OPEN_JTALK
	* @brief OpenJtalk形式
	*
	* \~english
	* @def AILIA_VOICE_DICTIONARY_TYPE_OPEN_JTALK
	* @brief Format for OpenJTalk
	*/
	public const Int32 AILIA_VOICE_DICTIONARY_TYPE_OPEN_JTALK = (0);

	/**
	* \~japanese
	* @def AILIA_VOICE_MODEL_TYPE_TACOTRON2
	* @brief Tacoreon2形式
	*
	* \~english
	* @def AILIA_VOICE_MODEL_TYPE_TACOTRON2
	* @brief Format for Tacotron2
	*/
	public const Int32 AILIA_VOICE_MODEL_TYPE_TACOTRON2 = (0);

	/**
	* \~japanese
	* @def AILIA_VOICE_MODEL_TYPE_GPT_SOVITS
	* @brief GPT-SoVITS形式
	*
	* \~english
	* @def AILIA_VOICE_MODEL_TYPE_GPT_SOVITS
	* @brief Format for GPT-SoVITS
	*/
	public const Int32 AILIA_VOICE_MODEL_TYPE_GPT_SOVITS = (1);

	/**
	* \~japanese
	* @def AILIA_VOICE_CLEANER_TYPE_BASIC
	* @brief BasicCleaner
	*
	* \~english
	* @def AILIA_VOICE_CLEANER_TYPE_BASIC
	* @brief BasicCleaner
	*/
	public const Int32 AILIA_VOICE_CLEANER_TYPE_BASIC = (0);

	/**
	* \~japanese
	* @def AILIA_VOICE_CLEANER_TYPE_ENGLISH
	* @brief EnglishCleaner
	*
	* \~english
	* @def AILIA_VOICE_CLEANER_TYPE_ENGLISH
	* @brief EnglishCleaner
	*/
	public const Int32 AILIA_VOICE_CLEANER_TYPE_ENGLISH = (1);

	/****************************************************************
	* フラグ定義
	**/

	/**
	* \~japanese
	* @def AILIA_VOICE_FLAG_NONE
	* @brief フラグを設定しません
	*
	* \~english
	* @def AILIA_VOICE_FLAG_NONE
	* @brief Default flag
	*/
	public const Int32 AILIA_VOICE_FLAG_NONE = (0);

	/****************************************************************
	* G2Pの後処理
	**/

	/**
	* \~japanese
	* @def AILIA_VOICE_TEXT_POST_PROCESS_NONE
	* @brief 何もしません
	*
	* \~english
	* @def AILIA_VOICE_TEXT_POST_PROCESS_NONE
	* @brief Default flag
	*/
	public const Int32 AILIA_VOICE_TEXT_POST_PROCESS_NONE = (0);

	/**
	* \~japanese
	* @def AILIA_VOICE_TEXT_POST_PROCESS_REMOVE_SPACE
	* @brief SPACEを削除します
	*
	* \~english
	* @def AILIA_VOICE_TEXT_POST_PROCESS_REMOVE_SPACE
	* @brief Remove space
	*/
	public const Int32 AILIA_VOICE_TEXT_POST_PROCESS_REMOVE_SPACE = (1);

	/**
	* \~japanese
	* @def AILIA_VOICE_TEXT_POST_PROCESS_APPEND_PUNCTUATION
	* @brief 句読点を追加します。
	*
	* \~english
	* @def AILIA_VOICE_TEXT_POST_PROCESS_APPEND_PUNCTUATION
	* @brief Add punctuation
	*/
	public const Int32 AILIA_VOICE_TEXT_POST_PROCESS_APPEND_PUNCTUATION = (2);

	/**
	* \~japanese
	* @def AILIA_VOICE_TEXT_POST_PROCESS_APPEND_ACCENT
	* @brief アクセントを追加します
	*
	* \~english
	* @def AILIA_VOICE_TEXT_POST_PROCESS_APPEND_ACCENT
	* @brief Add accent
	*/
	public const Int32 AILIA_VOICE_TEXT_POST_PROCESS_APPEND_ACCENT = (4);

	/****************************************************************
	* APIコールバック定義
	**/

	public delegate int ailiaCallbackAudioResample(IntPtr a, IntPtr b, int c, int d, int e, int f);
	public delegate int ailiaCallbackAudioGetResampleLen(IntPtr a, int b, int c, int d);
	public delegate int ailiaCallbackCreate(IntPtr a, int b, int c);
	public delegate int ailiaCallbackOpenWeightFileA(IntPtr a, IntPtr b);
	public delegate int ailiaCallbackOpenWeightFileW(IntPtr a, IntPtr b);
	public delegate int ailiaCallbackOpenWeightMem(IntPtr a, IntPtr b, UInt32 c);
	public delegate int ailiaCallbackSetMemoryMode(IntPtr a, UInt32 b);
	public delegate void ailiaCallbackDestroy(IntPtr a);
	public delegate int ailiaCallbackUpdate(IntPtr a);
	public delegate int ailiaCallbackGetBlobIndexByInputIndex(IntPtr a, IntPtr b, uint c);
	public delegate int ailiaCallbackGetBlobIndexByOutputIndex(IntPtr a, IntPtr b, uint c);
	public delegate int ailiaCallbackGetBlobData(IntPtr a, IntPtr b, uint c, uint d);
	public delegate int ailiaCallbackSetInputBlobData(IntPtr a, IntPtr b, uint c, uint d);
	public delegate int ailiaCallbackSetInputBlobShape(IntPtr a, Ailia.AILIAShape  b, uint c, uint d);
	public delegate int ailiaCallbackGetBlobShape(IntPtr a, IntPtr b, uint c, uint d);
	public delegate int ailiaCallbackGetInputBlobCount(IntPtr a, IntPtr b);
	public delegate int ailiaCallbackGetOutputBlobCount(IntPtr a, IntPtr b);
	public delegate IntPtr ailiaCallbackGetErrorDetail(IntPtr a);

	/****************************************************************
	 *  引数をスルーする系のAPIに変換
	 **/

	[DllImport(Ailia.LIBRARY_NAME)]
	public static extern int ailiaCreate(IntPtr net, int env_id, int num_thread);
	[DllImport(Ailia.LIBRARY_NAME)]
	public static extern int ailiaOpenWeightFileW(IntPtr net, IntPtr path);
	[DllImport(Ailia.LIBRARY_NAME)]
	public static extern int ailiaOpenWeightFileA(IntPtr net, IntPtr path);
	[DllImport(Ailia.LIBRARY_NAME)]
	public static extern int ailiaOpenWeightMem(IntPtr net, IntPtr buf, uint buf_size);   
	[DllImport(Ailia.LIBRARY_NAME)]
	public static extern int ailiaGetBlobIndexByInputIndex(IntPtr net, IntPtr blob_idx, UInt32 input_blob_idx);
	[DllImport(Ailia.LIBRARY_NAME)]
	public static extern int ailiaGetBlobIndexByOutputIndex(IntPtr net, IntPtr blob_idx, UInt32 output_blob_idx);
	[DllImport(Ailia.LIBRARY_NAME)]
	public static extern int ailiaGetBlobShape(IntPtr net, IntPtr shape, UInt32 blob_idx, UInt32 version);
	[DllImport(Ailia.LIBRARY_NAME)]
	public static extern int ailiaGetInputBlobCount(IntPtr net, IntPtr shape);
	[DllImport(Ailia.LIBRARY_NAME)]
	public static extern int ailiaGetOutputBlobCount(IntPtr net, IntPtr shape);
	[DllImport(AiliaAudio.LIBRARY_NAME)]
	public static extern int ailiaAudioResample(IntPtr a, IntPtr b, int c, int d, int e, int f);
	[DllImport(AiliaAudio.LIBRARY_NAME)]
	public static extern int ailiaAudioGetResampleLen(IntPtr a, int b, int c, int d);

	/****************************************************************
	 *  IL2CPP用
	 *  以下のエラーを抑制するために、一度、C#空間でブリッジする
	 *  NotSupportedException: To marshal a managed method, please add an attribute named 'MonoPInvokeCallback' to the method definition.
	 **/

	[AOT.MonoPInvokeCallback(typeof(ailiaCallbackAudioResample))]
	public static int ailiaCallbackAudioResampleBridge (IntPtr a, IntPtr b, int c, int d, int e, int f) {
		return ailiaAudioResample(a, b, c, d, e, f);
	}

	[AOT.MonoPInvokeCallback(typeof(ailiaCallbackAudioGetResampleLen))]
	public static int ailiaCallbackAudioGetResampleLenBridge (IntPtr a, int b, int c, int d) {
		return ailiaAudioGetResampleLen(a, b, c, d);
	}

	[AOT.MonoPInvokeCallback(typeof(ailiaCallbackCreate))]
	public static int ailiaCallbackCreateBridge (IntPtr a, int b, int c) {
		return ailiaCreate(a, b, c);
	}

	[AOT.MonoPInvokeCallback(typeof(ailiaCallbackOpenWeightFileA))]
	public static int ailiaCallbackOpenWeightFileABridge (IntPtr a, IntPtr b) {
		return ailiaOpenWeightFileA(a, b);
	}

	[AOT.MonoPInvokeCallback(typeof(ailiaCallbackOpenWeightFileW))]
	public static int ailiaCallbackOpenWeightFileWBridge (IntPtr a, IntPtr b) {
		return ailiaOpenWeightFileW(a, b);
	}

	[AOT.MonoPInvokeCallback(typeof(ailiaCallbackOpenWeightMem))]
	public static int ailiaCallbackOpenWeightMemBridge (IntPtr a, IntPtr b, uint c) {
		return ailiaOpenWeightMem(a, b, c);
	}

	[AOT.MonoPInvokeCallback(typeof(ailiaCallbackSetMemoryMode))]
	public static int ailiaCallbackSetMemoryModeBridge (IntPtr a, uint b) {
		return Ailia.ailiaSetMemoryMode(a, b);
	}

	[AOT.MonoPInvokeCallback(typeof(ailiaCallbackDestroy))]
	public static void ailiaCallbackDestroyBridge (IntPtr a) {
		Ailia.ailiaDestroy(a);
	}

	[AOT.MonoPInvokeCallback(typeof(ailiaCallbackUpdate))]
	public static int ailiaCallbackUpdateBridge (IntPtr a) {
		return Ailia.ailiaUpdate(a);
	}

	[AOT.MonoPInvokeCallback(typeof(ailiaCallbackGetBlobIndexByInputIndex))]
	public static int ailiaCallbackGetBlobIndexByInputIndexBridge (IntPtr a, IntPtr b, uint c) {
		return ailiaGetBlobIndexByInputIndex(a, b, c);
	}

	[AOT.MonoPInvokeCallback(typeof(ailiaCallbackGetBlobIndexByOutputIndex))]
	public static int ailiaCallbackGetBlobIndexByOutputIndexBridge (IntPtr a, IntPtr b, uint c) {
		return ailiaGetBlobIndexByOutputIndex(a, b, c);
	}

	[AOT.MonoPInvokeCallback(typeof(ailiaCallbackGetBlobData))]
	public static int ailiaCallbackGetBlobDataBridge (IntPtr a, IntPtr b, uint c, uint d) {
		return Ailia.ailiaGetBlobData(a, b, c, d);
	}

	[AOT.MonoPInvokeCallback(typeof(ailiaCallbackSetInputBlobData))]
	public static int ailiaCallbackSetInputBlobDataBridge (IntPtr a, IntPtr b, uint c, uint d) {
		return Ailia.ailiaSetInputBlobData(a, b, c, d);
	}

	[AOT.MonoPInvokeCallback(typeof(ailiaCallbackSetInputBlobShape))]
	public static int ailiaCallbackSetInputBlobShapeBridge (IntPtr a, Ailia.AILIAShape  b, uint c, uint d) {
		return Ailia.ailiaSetInputBlobShape(a, b, c, d);
	}

	[AOT.MonoPInvokeCallback(typeof(ailiaCallbackGetBlobShape))]
	public static int ailiaCallbackGetBlobShapeBridge (IntPtr a, IntPtr b, uint c, uint d) {
		return ailiaGetBlobShape(a, b, c, d);
	}

	[AOT.MonoPInvokeCallback(typeof(ailiaCallbackGetInputBlobCount))]
	public static int ailiaCallbackGetInputBlobCountBridge (IntPtr a, IntPtr b) {
		return ailiaGetInputBlobCount(a, b);
	}

	[AOT.MonoPInvokeCallback(typeof(ailiaCallbackGetOutputBlobCount))]
	public static int ailiaCallbackGetOutputBlobCountBridge (IntPtr a, IntPtr b) {
		return ailiaGetOutputBlobCount(a, b);
	}

	[AOT.MonoPInvokeCallback(typeof(ailiaCallbackGetErrorDetail))]
	public static IntPtr ailiaCallbackGetErrorDetailBridge (IntPtr a) {
		return Ailia.ailiaGetErrorDetail(a);
	}

	/**
	* \~japanese
	* @def AILIA_SPEECH_API_CALLBACK_VERSION
	* @brief 構造体バージョン
	*
	* \~english
	* @def AILIA_SPEECH_API_CALLBACK_VERSION
	* @brief Struct version
	*/
	public const int AILIA_VOICE_API_CALLBACK_VERSION = (1);

	/* APIコールバック関数構造体 */
	[StructLayout(LayoutKind.Sequential)]
	public struct AILIAVoiceApiCallback
	{
		public ailiaCallbackAudioResample ailiaAudioResample;
		public ailiaCallbackAudioGetResampleLen ailiaAudioGetResampleLen;

		public ailiaCallbackCreate ailiaCreate;
		public ailiaCallbackOpenWeightFileA ailiaOpenWeightFileA;
		public ailiaCallbackOpenWeightFileW ailiaOpenWeightFileW;
		public ailiaCallbackOpenWeightMem ailiaOpenWeightMem;
		public ailiaCallbackSetMemoryMode ailiaSetMemoryMode;
		public ailiaCallbackDestroy ailiaDestroy;
		public ailiaCallbackUpdate ailiaUpdate;
		public ailiaCallbackGetBlobIndexByInputIndex ailiaGetBlobIndexByInputIndex;
		public ailiaCallbackGetBlobIndexByOutputIndex ailiaGetBlobIndexByOutputIndex;
		public ailiaCallbackGetBlobData ailiaGetBlobData;
		public ailiaCallbackSetInputBlobData ailiaSetInputBlobData;
		public ailiaCallbackSetInputBlobShape ailiaSetInputBlobShape;
		public ailiaCallbackGetBlobShape ailiaGetBlobShape;
		public ailiaCallbackGetInputBlobCount ailiaGetInputBlobCount;
		public ailiaCallbackGetOutputBlobCount ailiaGetOutputBlobCount;
		public ailiaCallbackGetErrorDetail ailiaGetErrorDetail;
	};

	public static AiliaVoice.AILIAVoiceApiCallback GetCallback(){
		AiliaVoice.AILIAVoiceApiCallback callback=new AiliaVoice.AILIAVoiceApiCallback();

		callback.ailiaAudioResample=ailiaCallbackAudioResampleBridge;
		callback.ailiaAudioGetResampleLen=ailiaCallbackAudioGetResampleLenBridge;
		callback.ailiaCreate=ailiaCallbackCreateBridge;
		callback.ailiaOpenWeightFileA=ailiaCallbackOpenWeightFileABridge;
		callback.ailiaOpenWeightFileW=ailiaCallbackOpenWeightFileWBridge;
		callback.ailiaOpenWeightMem=ailiaCallbackOpenWeightMemBridge;
		callback.ailiaSetMemoryMode=ailiaCallbackSetMemoryModeBridge;
		callback.ailiaDestroy=ailiaCallbackDestroyBridge;
		callback.ailiaUpdate=ailiaCallbackUpdateBridge;
		callback.ailiaGetBlobIndexByInputIndex=ailiaCallbackGetBlobIndexByInputIndexBridge;
		callback.ailiaGetBlobIndexByOutputIndex=ailiaCallbackGetBlobIndexByOutputIndexBridge;
		callback.ailiaGetBlobData=ailiaCallbackGetBlobDataBridge;
		callback.ailiaSetInputBlobData=ailiaCallbackSetInputBlobDataBridge;
		callback.ailiaSetInputBlobShape=ailiaCallbackSetInputBlobShapeBridge;
		callback.ailiaGetBlobShape=ailiaCallbackGetBlobShapeBridge;
		callback.ailiaGetInputBlobCount=ailiaCallbackGetInputBlobCountBridge;
		callback.ailiaGetOutputBlobCount=ailiaCallbackGetOutputBlobCountBridge;
		callback.ailiaGetErrorDetail=ailiaCallbackGetErrorDetailBridge;
		
		return callback;
	}

	/****************************************************************
	* Voice API
	**/

	/**
	* \~japanese
	* @brief ボイスオブジェクトを作成します。
	* @param net ボイスオブジェクトポインタへのポインタ
	* @param env_id 計算に利用する推論実行環境のID( ailiaGetEnvironment() で取得)  \ref AILIA_ENVIRONMENT_ID_AUTO にした場合は自動で選択する
	* @param num_thread スレッド数の上限(  \ref AILIA_MULTITHREAD_AUTO  にした場合は自動で設定)
	* @param memory_mode メモリモード(AILIA_MEMORY_MODE_*)
	* @param flag AILIA_Voice_FLAG_*の論理和
	* @param api_callback ailiaのAPIへのコールバック
	* @param version AILIA_VOICE_API_CALLBACK_VERSION
	* @return
	*   成功した場合は \ref AILIA_STATUS_SUCCESS 、そうでなければエラーコードを返す。
	* @details
	*   ボイスオブジェクトを作成します。
	*
	* \~english
	* @brief Creates a Voice instance.
	* @param net A pointer to the Voice instance pointer
	* @param env_id The ID of the inference backend used for computation (obtained by  ailiaGetEnvironment() ). It is selected automatically if  \ref AILIA_ENVIRONMENT_ID_AUTO  is specified.
	* @param num_thread The upper limit on the number of threads (It is set automatically if  \ref AILIA_MULTITHREAD_AUTO
	* @param memory_mode The memory mode (AILIA_MEMORY_MODE_*)
	* @param flag OR of AILIA_Voice_FLAG_*
	* @param api_callback The callback for ailia API
	* @param version AILIA_VOICE_API_CALLBACK_VERSION
	* is specified.)
	* @return
	*   If this function is successful, it returns  \ref AILIA_STATUS_SUCCESS , or an error code otherwise.
	* @details
	*   Creates a Voice instance.
	*/
	[DllImport(LIBRARY_NAME)]
	public static extern int ailiaVoiceCreate(ref IntPtr net, int env_id, int num_thread, int memory_mode, int flags, AILIAVoiceApiCallback callback, int version);

	/**
	* \~japanese
	* @brief 辞書を指定します。
	* @param net ネットワークオブジェクトポインタ
	* @param dictionary_path 辞書フォルダのパス名
	* @param dictionary_type AILIA_VOICE_DICTIONARY_TYPE_*
	* @return
	*   成功した場合は \ref AILIA_STATUS_SUCCESS 、そうでなければエラーコードを返す。
	*
	* \~english
	* @brief Set dictionary into a network instance.
	* @param net A network instance pointer
	* @param dictionary_path The path name to the dictionary folder 
	* @param dictionary_type AILIA_VOICE_DICTIONARY_TYPE_*
	* @return
	*   If this function is successful, it returns  \ref AILIA_STATUS_SUCCESS , or an error code otherwise.
	*/
//#if (UNITY_STANDALONE_WIN || UNITY_EDITOR_WIN)
//	[DllImport(LIBRARY_NAME, EntryPoint = "ailiaVoiceOpenDictionaryFileW", CharSet=CharSet.Unicode)]
//	public static extern int ailiaVoiceOpenDictionaryFile(IntPtr net, string encoder, string decoder,, string postnet, string waveglow, int model_type); // not implemented
//#else
	[DllImport(LIBRARY_NAME, EntryPoint = "ailiaVoiceOpenDictionaryFileA", CharSet=CharSet.Ansi)]
	public static extern int ailiaVoiceOpenDictionaryFile(IntPtr net, string dictionary_path, int dictionary_type);
//#endif

	/**
	* \~japanese
	* @brief モデルを指定します。
	* @param net ネットワークオブジェクトポインタ
	* @param encoder onnxファイルのパス名
	* @param decoder1 onnxファイルのパス名
	* @param decoder2 onnxファイルのパス名
	* @param wave onnxファイルのパス名
	* @param ssl onnxファイルのパス名
	* @param model_type AILIA_VOICE_MODEL_TYPE_*
	* @param cleaner_type AILIA_VOICE_CLEANER_TYPE_*
	* @return
	*   成功した場合は \ref AILIA_STATUS_SUCCESS 、そうでなければエラーコードを返す。
	*
	* \~english
	* @brief Set models into a network instance.
	* @param net A network instance pointer
	* @param encoder The path name to the onnx file
	* @param decoder1 The path name to the onnx file
	* @param decoder2 The path name to the onnx file
	* @param wave The path name to the onnx file
	* @param ssl The path name to the onnx file
	* @param model_type AILIA_VOICE_MODEL_TYPE_*
	* @param cleaner_type AILIA_VOICE_CLEANER_TYPE_*
	* @return
	*   If this function is successful, it returns  \ref AILIA_STATUS_SUCCESS , or an error code otherwise.
	*/

#if (UNITY_STANDALONE_WIN || UNITY_EDITOR_WIN)
	[DllImport(LIBRARY_NAME, EntryPoint = "ailiaVoiceOpenModelFileW", CharSet=CharSet.Unicode)]
	public static extern int ailiaVoiceOpenModelFile(IntPtr net, string encoder, string decoder1, string decoder2, string wave, string ssl, int model_type, int cleaner_type);
#else
	[DllImport(LIBRARY_NAME, EntryPoint = "ailiaVoiceOpenModelFileA", CharSet=CharSet.Ansi)]
	public static extern int ailiaVoiceOpenModelFile(IntPtr net, string encoder, string decoder1, string decoder2, string wave, string ssl, int model_type, int cleaner_type);
#endif

	/**
	* \~japanese
	* @brief G2Pを行います。
	* @param net ボイスオブジェクトポインタ
	* @param text テキスト(UTF8)
	* @param text_post_process AILIA_VOICE_TEXT_POST_PROCESS_*
	* @return
	*   成功した場合は \ref AILIA_STATUS_SUCCESS 、そうでなければエラーコードを返す。
	* @details
	*   認識した結果はailiaVoiceGetFeatures APIで取得します。
	*
	* \~english
	* @brief Perform g2p
	* @param net A Voice instance pointer
	* @param text Text(UTF8)
	* @param text_post_process AILIA_VOICE_TEXT_POST_PROCESS_*
	* @return
	*   If this function is successful, it returns  \ref AILIA_STATUS_SUCCESS , or an error code otherwise.
	* @details
	*   Get the result with ailiaVoiceGetFeatures API.
	*/
	[DllImport(LIBRARY_NAME)]
	public static extern int ailiaVoiceGraphemeToPhoneme(IntPtr net, IntPtr utf8, int post_process);

	/**
	* \~japanese
	* @brief ExtractFullContextを行います。
	* @param net ボイスオブジェクトポインタ
	* @param text テキスト(UTF8)
	* @return
	*   成功した場合は \ref AILIA_STATUS_SUCCESS 、そうでなければエラーコードを返す。
	* @details
	*   認識した結果はailiaVoiceGetFeaturesAPIで取得します。
	*
	* \~english
	* @brief Perform ExtractFullContext
	* @param net A Voice instance pointer
	* @param text Text (UTF8)
	* @return
	*   If this function is successful, it returns  \ref AILIA_STATUS_SUCCESS , or an error code otherwise.
	* @details
	*   Get the result with ailiaVoiceGetFeatures API.
	*/
	[DllImport(LIBRARY_NAME)]
	public static extern int ailiaVoiceExtractFullContext(IntPtr net, IntPtr utf8);

	/**
	* \~japanese
	* @brief フィーチャーの長さを取得します。(NULL文字含む)
	* @param net   ボイスオブジェクトポインタ
	* @param len  フィーチャーの長さ
	* @return
	*   成功した場合は \ref AILIA_STATUS_SUCCESS 、そうでなければエラーコードを返す。
	*
	* \~english
	* @brief Gets the size of features. (Include null)
	* @param net   A Voice instance pointer
	* @param len  The length of features
	* @return
	*   If this function is successful, it returns  \ref AILIA_STATUS_SUCCESS , or an error code otherwise.
	*/
	[DllImport(LIBRARY_NAME)]
	public static extern int ailiaVoiceGetFeatureLength(IntPtr net, ref uint len);

	/**
	* \~japanese
	* @brief フィーチャーを取得します。
	* @param net   ボイスオブジェクトポインタ
	* @param features  フィーチャー(UTF8)
	* @param len バッファサイズ
	* @return
	*   成功した場合は \ref AILIA_STATUS_SUCCESS 、そうでなければエラーコードを返す。
	* @details
	*   ailiaVoiceGraphemeToPhoneme() もしくは ailiaVoiceExtractFullContext() を一度も実行していない場合は \ref AILIA_STATUS_INVALID_STATE が返ります。
	*
	* \~english
	* @brief Gets the decoded features.
	* @param net   A Voice instance pointer
	* @param features  Features(UTF8)
	* @param len  Buffer size
	* @return
	*   If this function is successful, it returns  \ref AILIA_STATUS_SUCCESS , or an error code otherwise.
	* @details
	*   If  ailiaVoiceGraphemeToPhoneme()  or ailiaVoiceExtractFullContext() is not run at all, the function returns  \ref AILIA_STATUS_INVALID_STATE .
	*/
	[DllImport(LIBRARY_NAME)]
	public static extern int ailiaVoiceGetFeatures(IntPtr net, IntPtr features, uint len);

	/**
	* \~japanese
	* @brief 0ショット音声合成のリファレンスとなる波形とテキストを設定します。
	* @param net   ボイスオブジェクトポインタ
	* @param buf  PCM波形 (0 - 1で正規化)
	* @param buf_size バッファサイズ（byte単位）
	* @param channels チャンネル数
	* @param sampling_rate サンプリングレート
	* @param features フィーチャー(UTF8)
	* 
	* @return
	*   成功した場合は \ref AILIA_STATUS_SUCCESS 、そうでなければエラーコードを返す。
	*
	* \~english
	* @brief Set the waveform and text as references for zero-shot voice synthesis.
	* @param net   A Voice instance pointer
	* @param buf   PCM Wave (Normalized by 0 - 1)
	* @param buf_size buffer size (byte unit)
	* @param channels num channels
	* @param sampling_rate sampling rate
	* @param features Feature (UTF8)
	* @return
	*   If this function is successful, it returns  \ref AILIA_STATUS_SUCCESS , or an error code otherwise.
	*/
	[DllImport(LIBRARY_NAME)]
	public static extern int ailiaVoiceSetReference(IntPtr net, IntPtr buf, uint buf_size, uint channels, uint sampling_rate, IntPtr features);

	/**
	* \~japanese
	* @brief 推論を行います。
	* @param net ボイスオブジェクトポインタ
	* @param text テキスト(UTF8)
	* @return
	*   成功した場合は \ref AILIA_STATUS_SUCCESS 、そうでなければエラーコードを返す。
	* @details
	*   認識した結果はailiaVoiceGetFeatures APIで取得します。
	*
	* \~english
	* @brief Perform inference
	* @param net A Voice instance pointer
	* @param text Text(UTF8)
	* @return
	*   If this function is successful, it returns  \ref AILIA_STATUS_SUCCESS , or an error code otherwise.
	* @details
	*   Get the result with ailiaVoiceGetWave API.
	*/
	[DllImport(LIBRARY_NAME)]
	public static extern int ailiaVoiceInference(IntPtr net, IntPtr utf8);

	/**
	* \~japanese
	* @brief 波形の情報を取得します。
	* @param net   ボイスオブジェクトポインタ
	* @param samples  サンプル数（チャンネル単位）
	* @param channels  チャンネル数
	* @param sampling_rate  サンプリングレート
	* @return
	*   成功した場合は \ref AILIA_STATUS_SUCCESS 、そうでなければエラーコードを返す。
	*
	* \~english
	* @brief Gets the information of wave.
	* @param net   A Voice instance pointer
	* @param samples  Number of samples (per channel)
	* @param channels  Number of channels
	* @param sampling_rate  Sampling rate
	* @return
	*   If this function is successful, it returns  \ref AILIA_STATUS_SUCCESS , or an error code otherwise.
	*/
	[DllImport(LIBRARY_NAME)]
	public static extern int ailiaVoiceGetWaveInfo(IntPtr net, ref uint samples, ref uint channels, ref uint sampling_rate);

	/**
	* \~japanese
	* @brief 波形を取得します。
	* @param net   ボイスオブジェクトポインタ
	* @param buf  PCM波形
	* @param buf_size バッファサイズ（byte単位）
	* @return
	*   成功した場合は \ref AILIA_STATUS_SUCCESS 、そうでなければエラーコードを返す。
	* @details
	*   ailiaVoiceInference() を一度も実行していない場合は \ref AILIA_STATUS_INVALID_STATE が返ります。
	*
	* \~english
	* @brief Gets the decoded features.
	* @param net   A Voice instance pointer
	* @param buf   PCM Wave
	* @param buf_size  Buffer size (Byte unit)
	* @return
	*   If this function is successful, it returns  \ref AILIA_STATUS_SUCCESS , or an error code otherwise.
	* @details
	*   If  ailiaVoiceInference() is not run at all, the function returns  \ref AILIA_STATUS_INVALID_STATE .
	*/
	[DllImport(LIBRARY_NAME)]
	public static extern int ailiaVoiceGetWave(IntPtr net, IntPtr buf, uint buf_size);

	/**
	* \~japanese
	* @brief ボイスオブジェクトを破棄します。
	* @param net ボイスオブジェクトポインタ
	*
	* \~english
	* @brief It destroys the Voice instance.
	* @param net A Voice instance pointer
	*/
	[DllImport(LIBRARY_NAME)]
	public static extern void ailiaVoiceDestroy(IntPtr net);

	/**
	* \~japanese
	* @brief エラーの詳細を返します
	* @param net   ネットワークオブジェクトポインタ
	* @return
	*   エラー詳細
	* @details
	*   返値は解放する必要はありません。
	*   文字列の有効期間は次にailiaVoiceのAPIを呼ぶまでです。
	*   取得したポイントから以下のように文字列に変換して下さい。
	*   @code
	*   Marshal.PtrToStringAnsi(Ailia.ailiaGetErrorDetail(net))
	*   @endcode
	*
	* \~english
	* @brief Returns the details of errors.
	* @param net   The network instance pointer
	* @return
	*   Error details
	* @details
	*   The return value does not have to be released.
	*   The string is valid until the next ailiaVoice API function is called.
	*   Convert from the point obtained to a string as follows
	*   @code
	*   Marshal.PtrToStringAnsi(Ailia.ailiaGetErrorDetail(net))
	*   @endcode
	*/
	[DllImport(LIBRARY_NAME)]
	public static extern IntPtr ailiaVoiceGetErrorDetail(IntPtr net);
}
} // namespace ailiaVoice