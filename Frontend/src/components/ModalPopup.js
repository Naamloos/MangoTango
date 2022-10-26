import { Button, Modal } from "react-bootstrap";
import React from "react";

class ModalPopup extends React.Component {
  constructor(props) {
    super(props);
    this.state = { title: "", message: "", visible: false };

    props.register(this.showModal.bind(this));
  }

  showModal(title, message) {
    this.setState({ title: title, message: message, visible: true });
  }

  render() {
    return (
      <Modal
        show={this.state.visible}
        onHide={() => {
          this.setState({ modal: false });
        }}
        centered
        backdrop="static"
        keyboard={false}
      >
        <Modal.Header>
          <Modal.Title>{this.state.title}</Modal.Title>
        </Modal.Header>
        <Modal.Body>{this.state.message}</Modal.Body>
        <Modal.Footer>
          <Button
            variant="secondary"
            onClick={() => {
              this.setState({ visible: false });
            }}
          >
            Close
          </Button>
        </Modal.Footer>
      </Modal>
    );
  }
}

export default ModalPopup;
