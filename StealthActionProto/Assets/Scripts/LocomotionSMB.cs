using UnityEngine;

public class LocomotionSMB : StateMachineBehaviour
{
    public float m_Damping = 0.15f;


    private readonly int m_HashHorizontalPara = Animator.StringToHash("Horizontal");
    private readonly int m_HashVerticalPara = Animator.StringToHash("Vertical");

    private Vector2 input = new Vector2(1.0f, 0.0f);
    private bool isAnimRun = false;

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");

        Vector2 input_int = new Vector2(horizontal, vertical).normalized;
        if (!(input_int.x == 0.0f && input_int.y == 0.0f))
        {
            input.Set(input_int.x, input_int.y);
        }
        //Debug.Log("hey");

        animator.SetFloat(m_HashHorizontalPara, input.x, m_Damping, Time.deltaTime);
        animator.SetFloat(m_HashVerticalPara, input.y, m_Damping, Time.deltaTime);
    }
}