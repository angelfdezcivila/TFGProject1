using System;
using Events;
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
        private Button _startButton;
        [SerializeField] 
        private Slider _multiplierSpeedSlider; // Intentar hacer mientras se ejecuta se pueda tocar la velocidad de la simulación y que se pueda ir marcha atras

        private void Awake()
        {
            _startButton.onClick.AddListener(startOnClick);
        }

        // Con eventos. Uso esta implementación debido a que desde el principio vamos a tener rellenados los datos en la UI
        // Este método va a controlar si están rellenos y lo lanzará en caso de que estén correctos los datos introducidos.
        private void startOnClick()
        {
            bool parametersValid = _pedestrianVelocityInputField.text.Length > 0;
            // if(_pedestrianVelocityInputField.contentType == TMP_InputField.ContentType.DecimalNumber)
            if (parametersValid)
            {
                ParametersEvents.OnUpdateStageParameters?.Invoke((float) Double.Parse(_pedestrianVelocityInputField.text), _multiplierSpeedSlider.value);
                ParametersEvents.OnPlaySimulation?.Invoke();
            }
        }
        
    }
}