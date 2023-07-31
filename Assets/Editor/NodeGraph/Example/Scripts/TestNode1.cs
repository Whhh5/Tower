using NodeGraph;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

public class TestNode1Data : NodeBaseData
{
    public string content;

    public TestNode1Data()
    {
        NodeName = "TestNode1";
    }
}

public class TestNode1 : NodeBase<TestNode1, TestNode1Data>, INode
{
    public readonly Vector2 _defaultNodeSize = new Vector2(150, 200);


    public override void OnCreated(NodeGraphView view)
    {
        if (view == null) return;
        title = NodeName;

        var textField = new TextField("Content", -1, true, false, default);
        textField.RegisterValueChangedCallback(evt =>
        {
            data.content = evt.newValue;
        });
        textField.SetValueWithoutNotify(name);
        textField.value = $"{data?.content}";
        mainContainer.Add(textField);

        var inputPort = GeneratePort(this, Direction.Input, typeof(string), Port.Capacity.Single);
        inputPort.portName = "Input";
        inputPort.portType = typeof(string);
        inputContainer.Add(inputPort);


        var outputPort = GeneratePort(this, Direction.Output, typeof(string), Port.Capacity.Single);
        outputPort.portName = "Output";
        outputContainer.Add(outputPort);


        var outputPort2 = GeneratePort(this, Direction.Output, typeof(string), Port.Capacity.Single);
        outputPort2.portName = "Output";
        outputPort2.portType = typeof(string);
        outputContainer.Add(outputPort2);


        var outputPort3 = GeneratePort(this, Direction.Output, typeof(string), Port.Capacity.Single);
        outputPort3.portName = "Output";
        outputPort3.portType = typeof(string);
        outputContainer.Add(outputPort3);

        var outputPort4 = GeneratePort(this, Direction.Output, typeof(string), Port.Capacity.Single);
        outputPort4.portName = "Output";
        outputPort4.portType = typeof(string);
        outputContainer.Add(outputPort4);



        var classList = inputContainer.GetClasses();


        RefreshExpandedState();
        RefreshPorts();
        SetPosition(new Rect(new Vector2(0, 0), _defaultNodeSize));
    }
}
