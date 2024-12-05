from flask import Flask, request, jsonify
from transformers import T5ForConditionalGeneration, T5Tokenizer

app = Flask(__name__)

model_name = "t5-small"
model = T5ForConditionalGeneration.from_pretrained(model_name)
tokenizer = T5Tokenizer.from_pretrained(model_name)

@app.route("/summarize", methods=["POST"])
def summarize():
    data = request.json
    text = data.get("text", "")

    if not text:
        return jsonify({"error": "No text provided"}), 400
    
    input_ids = tokenizer.encode("summarize: " + text, return_tensors="pt", truncation=True)

    outputs = model.generate(input_ids, max_length=250, min_length=30, length_penalty=2.0, num_beams=4, early_stopping=True)
    summary = tokenizer.decode(outputs[0], skip_special_tokens=True)

    return jsonify({"summary": summary})

if __name__ == "__main__":
    app.run(host="0.0.0.0", port=5000)