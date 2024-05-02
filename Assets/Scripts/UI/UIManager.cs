using System;
using Events;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class UIManager : MonoBehaviour
    {
        // TODO: Todo lo que sean números que no pueden ser negativos, plantear si hacerlo mediante sliders
        // Es posible que se tenga que hacer una referencia al InitializateStage en lugar de eventos
        [SerializeField] 
        private TMP_InputField _cellDimensionInputField;
        [SerializeField] 
        private TMP_InputField _pedestrianVelocityInputField;

        #region Trace save or load

        [SerializeField] 
        private Button _traceSaveOrLoadFolderButton;
        [SerializeField] 
        private Toggle _traceSaveOrLoadToggle;

        #endregion
        
        #region Stage save or load

        [SerializeField] 
        private Button _stageSaveOrLoadFolderButton;
        [SerializeField] 
        private Toggle _stageSaveOrLoadToggle;

        #endregion
        
        [SerializeField] 
        private Toggle _upsideViewToggle;
        [SerializeField] 
        private Slider _multiplierSpeedSlider; // Intentar hacer mientras se ejecuta se pueda tocar la velocidad de la simulación y que se pueda ir marcha atras
        [SerializeField] 
        private Button _randomStageButton;
        [SerializeField] 
        private Button _startButton;

        private float CellsDimensions => float.Parse(_cellDimensionInputField.text);
        private float PedestriansVelocity => float.Parse(_pedestrianVelocityInputField.text);
        private float MultiplierSpeed => _multiplierSpeedSlider.value;
        
        private float fixedDeltaTime;

        private void Awake()
        {
            _cellDimensionInputField.onEndEdit.AddListener(text => OnDecimalFieldChanged(_cellDimensionInputField));
            _pedestrianVelocityInputField.onEndEdit.AddListener(text => OnDecimalFieldChanged(_pedestrianVelocityInputField));
            
            _traceSaveOrLoadFolderButton.onClick.AddListener(OpenTraceFileExplorer);
            _traceSaveOrLoadToggle.onValueChanged.AddListener(checkActive => TogglingSaveAndUpdate(checkActive, TypeJsonButton.Trace));
            _stageSaveOrLoadFolderButton.onClick.AddListener(OpenStageFileExplorer);
            _stageSaveOrLoadToggle.onValueChanged.AddListener(checkActive => TogglingSaveAndUpdate(checkActive, TypeJsonButton.Stage));
            _upsideViewToggle.onValueChanged.AddListener(TogglingView);
            _randomStageButton.onClick.AddListener(GenerateRandomStage);
            _startButton.onClick.AddListener(StartOnClick);

            _multiplierSpeedSlider.onValueChanged.AddListener(value => SimulationEvents.OnUpdateSimulationSpeed?.Invoke(value));
        }

        private void TogglingSaveAndUpdate(bool savingJson, TypeJsonButton type)
        {
            string path = "";
            switch (type)
            {
                case TypeJsonButton.Trace : 
                    _traceSaveOrLoadFolderButton.GetComponentInChildren<TextMeshProUGUI>().text = savingJson ? "Guardar traza en" : "Cargar traza";
                    path = savingJson ? PathsForJson.SaveTraceJson : PathsForJson.LoadTraceJson;
                    break;
                case TypeJsonButton.Stage :
                    _stageSaveOrLoadFolderButton.GetComponentInChildren<TextMeshProUGUI>().text = savingJson ? "Guardar escenario en" : "Cargar escenario";
                    path = savingJson ? PathsForJson.SaveStageJson : PathsForJson.LoadStageJson;
                    break;
            }
            FileExplorerEvents.OnSelectedPathForJson?.Invoke(path, type, savingJson);
        }
        
        private void TogglingView(bool upsideView)
        {
            CameraEvents.OnTogglingView?.Invoke(upsideView);
        }

        private void OpenTraceFileExplorer() => OpenFileExplorer(_traceSaveOrLoadToggle.isOn, TypeJsonButton.Trace);
        private void OpenStageFileExplorer() => OpenFileExplorer(_stageSaveOrLoadToggle.isOn, TypeJsonButton.Stage);

        private void OpenFileExplorer(bool toggleIsOn, TypeJsonButton type)
        {
            FileExplorerEvents.OnOpenFileExplorer?.Invoke(toggleIsOn, type);
        }

        // Con eventos. Uso esta implementación debido a que desde el principio vamos a tener rellenados los datos en la UI
        // Este método va a controlar si están rellenos y lo lanzará en caso de que estén correctos los datos introducidos.
        private void StartOnClick()
        {
            bool parametersValid = _cellDimensionInputField.text.Length > 0 && _pedestrianVelocityInputField.text.Length > 0;
            // if(_pedestrianVelocityInputField.contentType == TMP_InputField.ContentType.DecimalNumber)
            if (parametersValid)
            {
                SimulationEvents.OnInitializeStageParameters?.Invoke(CellsDimensions, PedestriansVelocity, MultiplierSpeed);
                SimulationEvents.OnPlaySimulation?.Invoke(_traceSaveOrLoadToggle.isOn);
                _randomStageButton.interactable = false;
                
            }
        }
        
        private void GenerateRandomStage()
        {
            SimulationEvents.OnInitializeStageParameters?.Invoke(CellsDimensions, PedestriansVelocity, MultiplierSpeed);
            SimulationEvents.OnGenerateRandomStage?.Invoke();
        }


        //Para controlar el valor decimal de un input
        private void OnDecimalFieldChanged(TMP_InputField field)
        {
            var fieldText = field.text;
            var number = float.Parse(fieldText);
            if (number <= 0)
            {
                //TODO: Avisar de que el valor debe ser mayor a 0
                field.text = "";
            }
            else if (fieldText.Contains(','))
            {
                string[] charAfterComa = fieldText.Split(",");
                string strAfterComa = charAfterComa[1];

                if (strAfterComa.Length > 2)
                {
                    var truncated = Math.Truncate(number * 100) / 100;
                    fieldText = truncated.ToString();
                    field.text = fieldText;
                }
            }

        }
        
    }
}