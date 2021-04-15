import { useLocation } from "react-router";

export default function UseQuery() {
    return new URLSearchParams(useLocation().search);
}