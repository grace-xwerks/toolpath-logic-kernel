import sys
import json
import os
from PySide6 import QtWidgets
from NodeGraphQt import NodeGraph, BaseNode

# 1. Define the Global Machine Node (The Environment)
class MachineNode(BaseNode):
    __identifier__ = 'tlk.nodes'
    NODE_NAME = 'Machine Config'

    def __init__(self):
        super(MachineNode, self).__init__()
        self.add_input('Sync In', color=(180, 80, 80))
        self.add_output('Channel 1 State', color=(80, 180, 80))
        self.add_output('Channel 2 State', color=(80, 180, 80))
        self.add_text_input('machine_model', 'Model Name', text='Citizen_L20')
        self.add_checkbox('is_swiss', 'Swiss Kinematics', state=True)

# 2. Define an Action SubVI (The Toolpath Cycle)
class TurnCycleNode(BaseNode):
    __identifier__ = 'tlk.nodes'
    NODE_NAME = 'OD Turn Cycle'

    def __init__(self):
        super(TurnCycleNode, self).__init__()
        self.add_input('State In', color=(80, 180, 80))
        self.add_output('State Out', color=(80, 180, 80))
        self.add_text_input('target_dia', 'Diameter (X)', text='0.250')
        self.add_text_input('end_z', 'End Length (Z)', text='-0.500')
        self.add_combo_menu('tool_call', 'Tool', items=['T0101', 'T0202', 'T0303'])

# 3. Define the Synchronization Node (The Wait Code)
class SyncNode(BaseNode):
    __identifier__ = 'tlk.nodes'
    NODE_NAME = 'Wait / Sync'

    def __init__(self):
        super(SyncNode, self).__init__()
        self.add_input('Ch 1 In', color=(80, 180, 80))
        self.add_input('Ch 2 In', color=(80, 180, 80))
        self.add_output('Ch 1 Out', color=(80, 180, 80))
        self.add_output('Ch 2 Out', color=(80, 180, 80))
        self.add_text_input('sync_number', 'Sync Number', text='1')

# 4. Initialize the Application and Graph
def main():
    app = QtWidgets.QApplication(sys.argv)
    
    # --- UI WRAPPER SETUP ---
    main_window = QtWidgets.QWidget()
    main_window.setWindowTitle("TLK - Visual Kernel Alpha")
    main_window.resize(1100, 800)
    
    # Create a vertical layout to stack the graph and the button
    layout = QtWidgets.QVBoxLayout(main_window)
    layout.setContentsMargins(0, 0, 0, 0)
    
    graph = NodeGraph()
    graph.register_node(MachineNode)
    graph.register_node(TurnCycleNode)
    graph.register_node(SyncNode)
    
    # Add the graph widget to the top of the layout
    layout.addWidget(graph.widget)
    
    # --- EXPORT BUTTON LOGIC ---
    btn_export = QtWidgets.QPushButton("Compile & Export to JSON")
    btn_export.setMinimumHeight(50)
    btn_export.setStyleSheet("font-size: 16px; font-weight: bold; background-color: #2b2b2b; color: white;")
    layout.addWidget(btn_export)

    def execute_export():
        data = graph.serialize_session()
        
        # Route the JSON file directly to the schemas folder
        export_path = os.path.abspath(os.path.join(os.path.dirname(__file__), '..', 'tlk-schemas', 'active_program.json'))
        
        with open(export_path, 'w') as f:
            json.dump(data, f, indent=4)
            
        print(f"SUCCESS: Graph exported to {export_path}")

    # Wire the button to the function
    btn_export.clicked.connect(execute_export)
    
    # --- CANVAS POPULATION ---
    machine = graph.create_node('tlk.nodes.MachineNode', pos=[-200, 50])
    turn1 = graph.create_node('tlk.nodes.TurnCycleNode', pos=[200, -50])
    sync1 = graph.create_node('tlk.nodes.SyncNode', pos=[500, 50])
    
    machine.set_output(0, turn1.input(0))
    turn1.set_output(0, sync1.input(0))
    
    # Frame the camera
    for node in graph.all_nodes():
        node.set_selected(True)
    graph.fit_to_selection()
    graph.clear_selection()
    
    main_window.show()
    sys.exit(app.exec())

if __name__ == '__main__':
    main()