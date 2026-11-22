using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

public class TeleportationActivator : MonoBehaviour
{
    public XRRayInteractor teleportInteractor;
    public InputActionProperty teleportActivatorAction;

    void Start()
    {
        teleportInteractor.gameObject.SetActive(false);
    }

    void OnEnable()
    {
        teleportActivatorAction.action.performed += OnTeleportPressed;
        teleportActivatorAction.action.canceled += OnTeleportReleased;
    }

    void OnDisable()
    {
        teleportActivatorAction.action.performed -= OnTeleportPressed;
        teleportActivatorAction.action.canceled -= OnTeleportReleased;
    }

    private void OnTeleportPressed(InputAction.CallbackContext context)
    {
        teleportInteractor.gameObject.SetActive(true);
    }

    private void OnTeleportReleased(InputAction.CallbackContext context)
    {
        StartCoroutine(DisableNextFrame());
    }

    private IEnumerator DisableNextFrame()
    {
        yield return new WaitForEndOfFrame();
        if (teleportInteractor != null)
            teleportInteractor.gameObject.SetActive(false);
    }
}