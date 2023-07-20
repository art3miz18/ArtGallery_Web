using UnityEngine;
using System.Collections;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

namespace StarterAssets
{
	public class StarterAssetsInputs : MonoBehaviour
	{
		public delegate void CursorDelegate(Vector2 mousePosition);
		public static event CursorDelegate _mouseState;
		[Header("Character Input Values")]
		public Vector2 move;
		public Vector2 look;
		public bool jump;
		public bool sprint;
		public bool drag;
		public float TurnSpeed =0.1f;

		[Header("Movement Settings")]
		public bool analogMovement;

		[Header("Mouse Cursor Settings")]
		public bool cursorLocked = true;
		public bool cursorInputForLook = true;
		public Vector2 _cursorPosition;
		private Camera mainCam;
		private ThirdPersonController tpc;
		private float dragThreshold = 0.1f; // Define a threshold for drag to take effect
		private CursorManager cms;
		
		
#if ENABLE_INPUT_SYSTEM
		public void OnMove(InputValue value)
		{
			MoveInput(value.Get<Vector2>());
		}

		public void OnLook(InputValue value)
		{
			if(cursorInputForLook)
			{
				LookInput(value.Get<Vector2>());
			}
		}

		public void OnJump(InputValue value)
		{
			JumpInput(value.isPressed);
		}

		public void OnSprint(InputValue value)
		{
			SprintInput(value.isPressed);
		}
		public void OnPress(InputValue value){
			PressedInput(value.isPressed);
		}
		public void OnCursor(InputValue value){
			CursorPosition(value.Get<Vector2>());
		}
		public void OnMouseTap(InputValue value){
			Mousetap(value.isPressed);
		}
#endif

		private void Awake() {
			mainCam = Camera.main;
			tpc = GetComponent<ThirdPersonController>();
			cms = GetComponent<CursorManager>();			
		}
		
		IEnumerator Dragging()
		{
			drag = true;
			Vector2 prevPos = Vector2.zero;
			while (drag)
			{
				Vector2 currentPos = _cursorPosition;
				if (prevPos != Vector2.zero)
				{
					Vector2 dragVector = currentPos - prevPos;					
					if(dragVector.magnitude > dragThreshold)
					{
						tpc.LookInput +=  look * TurnSpeed;
					}
				}
				prevPos = currentPos;
				yield return null;
			}
			
			tpc.LookInput = Vector2.zero;
		}
		public void MoveInput(Vector2 newMoveDirection)
		{
			move = newMoveDirection;
		} 

		public void LookInput(Vector2 newLookDirection)
		{
			look = newLookDirection;			
		}

		public void JumpInput(bool newJumpState)
		{
			jump = newJumpState;
		}

		public void SprintInput(bool newSprintState)
		{
			sprint = newSprintState;
		}

		private void OnApplicationFocus(bool hasFocus)
		{
			SetCursorState(cursorLocked);
		}

		private void SetCursorState(bool newState)
		{
			Cursor.lockState = newState ? CursorLockMode.Locked : CursorLockMode.None;
		}
		private void PressedInput(bool dragging){
			cms.SetCursorTexture(cms.cursorTexture[2]);
			StartCoroutine(Dragging());
			drag = dragging;
			if(dragging == false){
				drag = false;
				cms.SetCursorTexture(cms.cursorTexture[0]);
			}
		}
		private void CursorPosition(Vector2 mousePosition){
			_cursorPosition = mousePosition;
		}

		private void Mousetap(bool currentState){
			if(drag == false){
				cms.RaycastTrigger();
				cms.SetCursorTexture(cms.cursorTexture[1]);
				StartCoroutine(HideAfterDelay());			
			}
		}

		private IEnumerator HideAfterDelay() 
		{
			yield return new WaitForSeconds(0.15f);
			cms.SetCursorTexture(cms.cursorTexture[0]);			
		}	
		
		private void Update(){
			_mouseState?.Invoke(_cursorPosition);
		}		
	}	
}