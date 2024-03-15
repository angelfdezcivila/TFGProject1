using System;
using Events;
using SimpleFileBrowser;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class UIManager : MonoBehaviour
    {
        // TODO: Todo lo que sean números que no pueden ser negativos, hacerlo mediante sliders
        // Es posible que se tenga que hacer una referencia al InitializateStage en lugar de eventos
        [SerializeField] 
        private TMP_InputField _pedestrianVelocityInputField;
        [SerializeField] 
        private Button _saveFolderButton;
        [SerializeField] 
        private Toggle _saveOrLoadToggle;
        [SerializeField] 
        private Slider _multiplierSpeedSlider; // Intentar hacer mientras se ejecuta se pueda tocar la velocidad de la simulación y que se pueda ir marcha atras
        [SerializeField] 
        private Button _startButton;
        
        private float fixedDeltaTime;

        private void Awake()
        {
            _saveFolderButton.onClick.AddListener(OpenFileExplorer);
            _saveOrLoadToggle.onValueChanged.AddListener(TogglingSaveAndUpdate);
            _startButton.onClick.AddListener(StartOnClick);
            
            // _multiplierSpeedSlider.onValueChanged.AddListener(UpdateTimeScale);
        }

        private void UpdateTimeScale(float multiplierSpeed)
        {
            if (multiplierSpeed >= 0)
            {
                Time.timeScale = multiplierSpeed;
                // Time.fixedDeltaTime = this.fixedDeltaTime * Time.timeScale;
            }
        }

        private void TogglingSaveAndUpdate(bool savingTrace)
        {
            _saveFolderButton.GetComponentInChildren<TextMeshProUGUI>().text = savingTrace ? "Guardar traza en" : "Cargar traza";
            string path = savingTrace ? InitializateStage.JsonSaveFilePath : "";
            FileExplorerEvents.OnSelectedPathForJson?.Invoke(path);
        }

        private void OpenFileExplorer()
        {
            FileExplorerEvents.OnOpenFileExplorer?.Invoke(_saveOrLoadToggle.isOn);
        }

        // Con eventos. Uso esta implementación debido a que desde el principio vamos a tener rellenados los datos en la UI
        // Este método va a controlar si están rellenos y lo lanzará en caso de que estén correctos los datos introducidos.
        private void StartOnClick()
        {
            bool parametersValid = _pedestrianVelocityInputField.text.Length > 0;
            // if(_pedestrianVelocityInputField.contentType == TMP_InputField.ContentType.DecimalNumber)
            if (parametersValid)
            {
                SimulationEvents.OnUpdateStageParameters?.Invoke((float) Double.Parse(_pedestrianVelocityInputField.text), _multiplierSpeedSlider.value);
                SimulationEvents.OnPlaySimulation?.Invoke(_saveOrLoadToggle.isOn);
            }
        }
        
    }
}