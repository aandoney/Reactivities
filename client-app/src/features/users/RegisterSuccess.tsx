import React from "react";
import { toast } from "react-toastify";
import { Button, Header, Icon, Segment } from "semantic-ui-react";
import agent from "../../app/api/agent";
import UseQuery from "../../app/common/util/hooks";

export default function RegisterSuccess() {
  const email = UseQuery().get("email") as string;

  function handleConfirmEmailResend() {
    agent.Account.resendEmailConfirm(email)
      .then(() => {
        toast.success("Verification email resent - please check you email");
      })
      .catch((error) => console.log(error));
  }

  return (
    <Segment placeholder textAlign="center">
      <Header icon color="green">
        <Icon name="check" />
        Successfully registered
      </Header>
      <p>
        Please check your email (including junk email) for the verification
        email
      </p>
      {email && (
        <>
          <p>Didn't receive the email? clich the below button to resend</p>
          <Button
            primary
            onClick={handleConfirmEmailResend}
            content="Resend email"
            size="huge"
          />
        </>
      )}
    </Segment>
  );
}
