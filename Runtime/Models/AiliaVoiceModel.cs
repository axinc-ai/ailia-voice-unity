/* ailia.voice model class */
/* Copyright 2024 AXELL CORPORATION */

using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using System.Threading;
using System.Runtime.InteropServices;

using ailia;

namespace ailiaVoice{
public class AiliaVoiceModel : IDisposable
{
	// instance
	private IntPtr net = IntPtr.Zero;
	private bool debug_log = true;
	private string audio_clip_name = "";
	private float[] audio_data = new float[0];
	private uint samples = 0;
	private uint channels = 0;
	private uint sampling_rate = 0;

    /****************************************************************
     * 実行環境の取得
     */

    private string env_name = "";

    /**
    * \~japanese
    * @brief 実行環境を取得します。
    * @param is_gpu   GPUを使用するかどうか
    * @return
    *   env_id
    *   
    * \~english
    * @brief Get the environmen id
    * @param is_gpu   Whether to use GPU
    * @return
    *   env_id
    */
    public int GetEnvironmentId(bool is_gpu){
        int env_id = Ailia.AILIA_ENVIRONMENT_ID_AUTO;
        if (is_gpu) { // GPU
            int count = 0;
            Ailia.ailiaGetEnvironmentCount(ref count);
            for (int i = 0; i < count; i++){
                IntPtr env_ptr = IntPtr.Zero;
                Ailia.ailiaGetEnvironment(ref env_ptr, (uint)i, Ailia.AILIA_ENVIRONMENT_VERSION);
                Ailia.AILIAEnvironment env = (Ailia.AILIAEnvironment)Marshal.PtrToStructure(env_ptr, typeof(Ailia.AILIAEnvironment));

                if (env.backend == Ailia.AILIA_ENVIRONMENT_BACKEND_MPS || env.backend == Ailia.AILIA_ENVIRONMENT_BACKEND_CUDA || env.backend == Ailia.AILIA_ENVIRONMENT_BACKEND_VULKAN){
                    env_id = env.id;
                    env_name = Marshal.PtrToStringAnsi(env.name);
                }
            }
        } else {
            env_name = "cpu";
        }
        return env_id;
    }

    /**
    * \~japanese
    * @brief 実行環境の名称を取得します。
    * @return
    *   env_name
    *   
    * \~english
    * @brief Get the environmen name
    * @return
    *   env_name
    */
    public string GetEnvironmentName(){
        return env_name;
    }

	/****************************************************************
	 * モデル
	 */

	/**
	* \~japanese
	* @brief インスタンスを作成します。
	* @param env_id         Ailiaの実行環境
	* @param flag           フラグの論理和（AiliaVoice.AILIA_VOICE_FLAG_*)
	* @return
	*   成功した場合はtrue、失敗した場合はfalseを返す。
	*   
	* \~english
	* @brief   Create a instance.
	* @param env_id         Environment ID of ailia
	* @param flag           OR of flags (AiliaVoice.AILIA_VOICE_FLAG_*)
	* @return
	*   If this function is successful, it returns  true  , or  false  otherwise.
	*/
	public bool Create(int env_id, int flag){
		if (net != null){
			Close();
		}

		AiliaVoice.AILIAVoiceApiCallback callback = AiliaVoice.GetCallback();

		int memory_mode = Ailia.AILIA_MEMORY_REDUCE_CONSTANT | Ailia.AILIA_MEMORY_REDUCE_CONSTANT_WITH_INPUT_INITIALIZER | Ailia.AILIA_MEMORY_REUSE_INTERSTAGE;
		int status = AiliaVoice.ailiaVoiceCreate(ref net, env_id, Ailia.AILIA_MULTITHREAD_AUTO, memory_mode, flag, callback, AiliaVoice.AILIA_VOICE_API_CALLBACK_VERSION);
		if (status != 0){
			if (debug_log){
				Debug.Log("ailiaVoiceCreate failed " + status);
			}
			return false;
		}

		return true;
	}

	/**
	* \~japanese
	* @brief 辞書を指定します。
	* @param dictionary_path 辞書フォルダのパス名
	* @param dictionary_type AILIA_VOICE_DICTIONARY_TYPE_*
	* @return
	*   成功した場合はtrue、失敗した場合はfalseを返す。
	*
	* \~english
	* @param net A network instance pointer
	* @param dictionary_path The path name to the dictionary folder 
	* @param dictionary_type AILIA_VOICE_DICTIONARY_TYPE_*
	* @return
	*   If this function is successful, it returns  true  , or  false  otherwise.
	*/
	public bool OpenDictionary(string dict_path, int dict_type){
		int status = AiliaVoice.ailiaVoiceOpenDictionaryFile(net, dict_path, dict_type);
		if (status != 0){
			if (debug_log){
				Debug.Log("ailiaVoiceOpenDictionaryFile faield " + status);
			}
			return false;
		}
		return true;
	}

	/**
	* \~japanese
	* @brief モデルを指定します。
	* @param encoder onnxファイルのパス名
	* @param decoder1 onnxファイルのパス名
	* @param postnet onnxファイルのパス名
	* @param wave onnxファイルのパス名
	* @param ssl onnxファイルのパス名
	* @param model_type AILIA_VOICE_MODEL_TYPE_*
	* @param cleaner_type AILIA_VOICE_CLEANER_TYPE_*
	* @return
	*   成功した場合はtrue、失敗した場合はfalseを返す。
	*
	* \~english
	* @param net A network instance pointer
	* @param encoder The path name to the onnx file
	* @param decoder1 The path name to the onnx file
	* @param decoder2 The path name to the onnx file
	* @param wave The path name to the onnx file
	* @param ssl The path name to the onnx file
	* @param model_type AILIA_VOICE_MODEL_TYPE_*
	* @param cleaner_type AILIA_VOICE_CLEANER_TYPE_*
	* @return
	*   If this function is successful, it returns  true  , or  false  otherwise.
	*/
	public bool OpenModel(string encoder, string decoder1, string decoder2, string wave, string ssl, int model_type, int cleaner_type){
        AiliaLicense.CheckAndDownloadLicense();

		int status = AiliaVoice.ailiaVoiceOpenModelFile(net, encoder, decoder1, decoder2, wave, ssl, model_type, cleaner_type);
		if (status != 0){
			if (debug_log){
				Debug.Log("ailiaVoiceOpenModelFile faield " + status);
			}
			return false;
		}
		return true;
	}

	/****************************************************************
	 * 開放する
	 */
	/**
	* \~japanese
	* @brief インスタンスを破棄します。
	* @details
	*   インスタンスを破棄し、初期化します。
	*   
	*  \~english
	* @brief   Destroys instance
	* @details
	*   Destroys and initializes the instance.
	*/
	public virtual void Close()
	{
		if (net != IntPtr.Zero){
			AiliaVoice.ailiaVoiceDestroy(net);
			net = IntPtr.Zero;
		}
	}

	/**
	* \~japanese
	* @brief リソースを解放します。
	*   
	*  \~english
	* @brief   Release resources.
	*/
	public virtual void Dispose()
	{
		Dispose(true);
	}

	protected virtual void Dispose(bool disposing)
	{
		if (disposing){
			// release managed resource
		}
		Close(); // release unmanaged resource
	}

	~AiliaVoiceModel(){
		Dispose(false);
	}

	/****************************************************************
	 * 音声合成
	 */

	/**
	* \~japanese
	* @brief 音素を取得します。
	* @param utf8    入力文字列
	* @param options ポストプロセスオプション
	* @return
	*   成功した場合はstring、失敗した場合は空文字を返す。
	*   
	* \~english
	* @brief   Get features
	* @param utf8    Input string
	* @param options Post process options
	* @return
	*   If this function is successful, it returns string  , or  empty string otherwise.
	*/
	public string G2P(string utf8, int options){
		byte[] text = System.Text.Encoding.UTF8.GetBytes(utf8+"\u0000");
		//Debug.Log(text[text.Length - 1]);
        GCHandle handle = GCHandle.Alloc(text, GCHandleType.Pinned);
        IntPtr input = handle.AddrOfPinnedObject();
		int status = AiliaVoice.ailiaVoiceGraphemeToPhoneme(net, input, options);
		handle.Free();
		if (status != 0){
			if (debug_log){
				Debug.Log("ailiaVoiceGraphemeToPhoneme faield " + status);
			}
			return "";
		}
		uint count = 0;
		status = AiliaVoice.ailiaVoiceGetFeatureLength(net, ref count);
		if (status != 0){
			if (debug_log){
				Debug.Log("ailiaVoiceGetFeatureLength faield " + status);
			}
			return "";
		}
		byte[] texts = new byte [count];
        handle = GCHandle.Alloc(texts, GCHandleType.Pinned);
        IntPtr output = handle.AddrOfPinnedObject();
		status = AiliaVoice.ailiaVoiceGetFeatures(net, output, count);
		handle.Free();
		if (status != 0){
			if (debug_log){
				Debug.Log("ailiaVoiceGetFeatures faield " + status);
			}
			return "";
		}
		return System.Text.Encoding.UTF8.GetString(texts);
	}

	/**
	* \~japanese
	* @brief リファレンス音声を設定します。
	* @param ref_audio    入力音声
	* @param ref_text    入力音声に対応するテキスト
	* @return
	*   成功した場合はAudioClip、失敗した場合はnullを返す。
	*   
	* \~english
	* @brief   Set reference audio
	* @param ref_audio    Reference audio
	* @param ref_text    Reference text
	* @return
	*   If this function is successful, it returns AudioClip  , or  null  otherwise.
	*/
	public bool SetReference(AudioClip ref_audio, string ref_text)
	{
		float[] audio_data = new float[ref_audio.samples * ref_audio.channels];
		ref_audio.GetData(audio_data, 0);

		GCHandle audio_handle = GCHandle.Alloc(audio_data, GCHandleType.Pinned);
		IntPtr audio_input = audio_handle.AddrOfPinnedObject();

		byte[] text = System.Text.Encoding.UTF8.GetBytes(ref_text);
		GCHandle text_handle = GCHandle.Alloc(text, GCHandleType.Pinned);
		IntPtr text_input = text_handle.AddrOfPinnedObject();
		int status = AiliaVoice.ailiaVoiceSetReference(net, audio_input, (uint)(ref_audio.samples * ref_audio.channels * 4), (uint)(ref_audio.channels), (uint)(ref_audio.frequency), text_input);
		text_handle.Free();
		audio_handle.Free();

		if (status != 0){
			if (debug_log){
				Debug.Log("ailiaVoiceSetReference faield " + status);
			}
			return false;
		}

		return true;
	}

	/**
	* \~japanese
	* @brief 音声合成を実行します。
	* @param feature    入力フューチャ
	* @return
	*   成功した場合はAtrue、失敗した場合はfalseを返す。
	*   
	* \~english
	* @brief   Perform inference
	* @param feature    Input feature string
	* @return
	*   If this function is successful, it returns true  , or  false  otherwise.
	*/
	public bool Inference(string feature)
	{
		byte[] text = System.Text.Encoding.UTF8.GetBytes(feature);
		GCHandle handle = GCHandle.Alloc(text, GCHandleType.Pinned);
		IntPtr input = handle.AddrOfPinnedObject();
		int status = AiliaVoice.ailiaVoiceInference(net, input);
		handle.Free();
		if (status != 0){
			if (debug_log){
				Debug.Log("ailiaVoiceInference faield " + status);
			}
			return false;
		}
		status = AiliaVoice.ailiaVoiceGetWaveInfo(net, ref samples, ref channels, ref sampling_rate);
		if (status != 0){
			if (debug_log){
				Debug.Log("ailiaVoiceGetWaveInfo faield " + status);
			}
			return false;
		}

		uint count = samples * channels;
		audio_data = new float [count];
		handle = GCHandle.Alloc(audio_data, GCHandleType.Pinned);
		IntPtr output = handle.AddrOfPinnedObject();
		status = AiliaVoice.ailiaVoiceGetWave(net, output, count * sizeof(float));
		handle.Free();
		if (status != 0){
			if (debug_log){
				Debug.Log("ailiaVoiceGetWave faield " + status);
			}
			return false;
		}

		audio_clip_name = feature;

		return true;
	}

	/**
	* \~japanese
	* @brief 音声合成結果を取得します。
	* @return
	*   成功した場合はAudioClip、失敗した場合はnullを返す。
	*   
	* \~english
	* @brief   Get audio clip
	* @return
	*   If this function is successful, it returns AudioClip  , or  null  otherwise.
	*/
	public AudioClip GetAudioClip()
	{
		AudioClip audioClip = AudioClip.Create(audio_clip_name, audio_data.Length, (int)channels, (int)sampling_rate, false);
		audioClip.SetData(audio_data, 0);
		return audioClip;
	}
}
} // namespace ailiaVoice
