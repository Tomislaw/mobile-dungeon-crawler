using RuinsRaiders;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;
using static RuinsRaiders.AStarSharp.Astar;

public class TestSceneTests
{
    protected const float waitTime = 1F;
    protected bool initialized = false;
    protected string ScenePath = "Assets/Scenes/Debug/TestScene";
    protected GameObject skelly1;
    protected GameObject skelly2;
    protected GameObject skelly3;
    protected GameObject skelly4;
    protected GameObject flyingSkelly;
    protected GameObject knight;
    protected MovementController mCtrl;
    protected PlayerController pCtrl;

    public Stack<RuinsRaiders.AStarSharp.Node> waitForPath(IEnumerator<FindPathStatus> enumerator)
    {
        Stack<RuinsRaiders.AStarSharp.Node> nodes = new Stack<RuinsRaiders.AStarSharp.Node>();
        while (enumerator.MoveNext())
        {
            if (enumerator.Current.Finished)
            {
                if (!enumerator.Current.PathFound)
                {
                    return nodes;
                }
                nodes = enumerator.Current.Path;
                break;
            }
        }
        return nodes;
    }

    //Wait for condition or timeout
    public sealed class WaitForDone : CustomYieldInstruction
    {
        private System.Func<bool> m_Predicate;
        private float m_timeout;
        private bool WaitForDoneProcess()
        {
            m_timeout -= Time.deltaTime;
            return m_timeout <= 0f || m_Predicate();
        }

        public override bool keepWaiting => !WaitForDoneProcess();

        public WaitForDone(float timeout, System.Func<bool> predicate)
        {
            m_Predicate = predicate;
            m_timeout = timeout;
        }
    }

    [UnitySetUp]
    public IEnumerator SetUp()
    {
        if (!initialized) //Only load the scene once!
        {
            yield return EditorSceneManager.LoadSceneAsyncInPlayMode($"{ScenePath}.unity", new LoadSceneParameters(LoadSceneMode.Single));
            skelly1 = GameObject.Find("/Skelly1");
            skelly2 = GameObject.Find("/Skelly2");
            skelly3 = GameObject.Find("/Skelly3");
            skelly4 = GameObject.Find("/Skelly4");
            flyingSkelly = GameObject.Find("/FlyingSkelly");
            knight = GameObject.Find("/Knight");
            mCtrl = knight.GetComponent<MovementController>();
            pCtrl = knight.GetComponent<PlayerController>();
            initialized = true;
        }
    }
}
