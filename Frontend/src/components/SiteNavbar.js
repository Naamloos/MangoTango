import { faGithub } from "@fortawesome/free-brands-svg-icons";
import { faRightFromBracket } from "@fortawesome/free-solid-svg-icons";
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import React from "react";
import { Container, Form, Navbar, Stack, Button } from "react-bootstrap";
import { Api } from "../utils/Api.js";

class SiteNavbar extends React.Component {
  constructor(props) {
    super(props);
    this.state = { rconPassword: "" };
  }

  handleLogin(e) {
    e.preventDefault();

    Api.checkAuth(
      this.state.rconPassword,
      () => {
        this.props.showModal(
          "Successfully authenticated!",
          "Your RCON password was correct. Welcome!"
        );
        this.props.authenticate(this.state.rconPassword);
      },
      () => {
        this.props.showModal(
          "Oops!",
          "Access denied. Your RCON password was incorrect."
        );
      }
    );
  }

  handleInput(e) {
    const name = e.target.id;
    this.setState({ [name]: e.target.value });
  }

  render() {
    return (
      <Navbar bg="dark" variant="dark" color="white" expand="lg" sticky="top">
        <Container className="p-1">
          <Navbar.Brand>{process.env.REACT_APP_NAME}</Navbar.Brand>
          <Navbar.Text>
            <Stack direction="horizontal" gap={3}>
              {this.props.authenticated ?
                (<Button variant="danger" onClick={this.props.logOut}>
                  <Stack gap={1} direction="horizontal">
                    <FontAwesomeIcon icon={faRightFromBracket} />
                    <>Log Out</>
                  </Stack>
                </Button>)
                :
                (<Form className="d-flex" onSubmit={this.handleLogin.bind(this)}>
                  <Form.Control
                    type="password"
                    id="rconPassword"
                    placeholder="Admin Login (RCON)"
                    onChange={this.handleInput.bind(this)}
                    disabled={this.props.authenticated}
                  />
                </Form>)}
              <a href="https://github.com/Naamloos/MangoTango" target="_">
                <FontAwesomeIcon
                  style={{ fontSize: "2em", color: "white", margin: 0 }}
                  icon={faGithub}
                />
              </a>
              <div></div>
            </Stack>
          </Navbar.Text>
        </Container>
      </Navbar>
    );
  }
}

export default SiteNavbar;
