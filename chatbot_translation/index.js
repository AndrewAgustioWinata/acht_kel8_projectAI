const { Telegraf, Markup } = require("telegraf");
const translator = require("translation-google");
const gtts = require("gtts");
const config = require("./config.json");
const fs = require("fs");
const https = require("https");
const Tesseract = require("tesseract.js");

const bot = new Telegraf(config.BOT_TOKEN);

const languages = {
    English: "en",
    Mandarin: "zh-CN",
    Japanese: "ja",
    "South Korean": "ko",
    Russian: "ru",
    Tagalog: "tl",
};

let selectedLanguage = "";
let inputMethod = "";

function generateLanguageMarkup() {
    const buttons = Object.keys(languages).map((language) =>
        Markup.button.callback(language, `select_language ${language}`)
    );
    return Markup.inlineKeyboard(buttons, { columns: 2 });
}

bot.start((ctx) => {
    ctx.replyWithHTML(
        "✨<b>Welcome GAN</b>✨!\nSelamat Datang di Bot Translation by.Kel8 🚀🚀.\n\nSilakan pilih bahasa yang ingin dijadikan terjemahan :\n(Oh iya, untuk sementara kita hanya menerima Bahasa Indonesia dulu ya. Ditunggu updatenya!)",
        generateLanguageMarkup()
    );
});

bot.action(/select_language (.+)/, (ctx) => {
    selectedLanguage = languages[ctx.match[1]];
    ctx.replyWithHTML(
        `Kamu memilih <b>${ctx.match[1]}</b>. Bagaimana kamu ingin mengirim teks yang ingin diterjemahkan?`,
        Markup.inlineKeyboard([
            Markup.button.callback("Teks", "text_input"),
            Markup.button.callback("Gambar", "image_input"),
        ])
    );
});

bot.action("text_input", (ctx) => {
    inputMethod = "text";
    ctx.replyWithHTML(
        `Silakan kirim teks yang ingin diterjemahkan ke <b>${getKeyByValue(
            languages,
            selectedLanguage
        )}</b> :`
    );
});

bot.action("image_input", (ctx) => {
    inputMethod = "image";
    ctx.reply(
        "Silakan kirim gambar yang berisi teks yang ingin diterjemahkan."
    );
});

bot.on("text", async (ctx) => {
    if (inputMethod !== "text") return;

    const text = ctx.update.message.text;

    if (!selectedLanguage) {
        ctx.reply("Dipilih dulu yaa bahasa yang diinginkan🤔🤔");
        return;
    }

    const loadingMessage = await ctx.reply("Sedang menerjemahkan...");

    const translation = await translator(text, {
        from: "auto",
        to: selectedLanguage,
    });

    await ctx.telegram.deleteMessage(ctx.chat.id, loadingMessage.message_id);

    ctx.replyWithHTML(
        `<b>Teks yang diterima</b> :\n${text}\n\n<b>Hasil translate (${getKeyByValue(
            languages,
            selectedLanguage
        )})</b> :\n${translation.text}`
    );

    const gttsInstance = new gtts(translation.text, selectedLanguage);
    const voiceFilePath = `./voice_${Date.now()}.mp3`;

    gttsInstance.save(voiceFilePath, (err) => {
        if (err) {
            console.error(err);
            ctx.reply("Terjadi kesalahan saat membuat voice note.");
        } else {
            ctx.replyWithVoice({ source: voiceFilePath }).then(() => {
                fs.unlinkSync(voiceFilePath);
                askForContinue(ctx);
            });
        }
    });
});

bot.on("photo", async (ctx) => {
    if (inputMethod !== "image") return;

    const loadingMessage = await ctx.reply("Sedang menerjemahkan...");

    const fileId = ctx.message.photo[ctx.message.photo.length - 1].file_id;
    const fileUrl = await bot.telegram.getFileLink(fileId);
    const imagePath = `./image_${Date.now()}.jpg`;

    await downloadFile(fileUrl, imagePath);

    const {
        data: { text },
    } = await Tesseract.recognize(imagePath, "eng");

    await ctx.telegram.deleteMessage(ctx.chat.id, loadingMessage.message_id);

    if (text.trim().length === 0) {
        ctx.reply("Tidak ada teks yang terdeteksi di gambar.");
        fs.unlinkSync(imagePath);
        return;
    }

    const joinedText = text.replace(/\n/g, " ");

    const translation = await translator(joinedText, {
        from: "auto",
        to: selectedLanguage,
    });

    ctx.replyWithHTML(
        `<b>Teks yang diterima dari gambar</b> :\n${joinedText}\n\n<b>Hasil translate (${getKeyByValue(
            languages,
            selectedLanguage
        )})</b> :\n${translation.text}`
    );

    const gttsInstance = new gtts(translation.text, selectedLanguage);
    const voiceFilePath = `./voice_${Date.now()}.mp3`;

    gttsInstance.save(voiceFilePath, (err) => {
        if (err) {
            console.error(err);
            ctx.reply("Terjadi kesalahan saat membuat voice note.");
        } else {
            ctx.replyWithVoice({ source: voiceFilePath }).then(() => {
                fs.unlinkSync(voiceFilePath);
                askForContinue(ctx);
            });
        }
    });

    fs.unlinkSync(imagePath);
});

function askForContinue(ctx) {
    ctx.reply(
        "Mau lanjut terjemahkan dengan bahasa yang sama atau ubah bahasa nih?",
        Markup.inlineKeyboard([
            Markup.button.callback("Lanjut", "again"),
            Markup.button.callback("Pilih bahasa lain", "different"),
        ])
    );
}

bot.action("again", (ctx) => {
    ctx.replyWithHTML(
        `Bagaimana kamu ingin mengirim teks yang ingin diterjemahkan ke <b>${getKeyByValue(
            languages,
            selectedLanguage
        )}</b> :`,
        Markup.inlineKeyboard([
            Markup.button.callback("Teks", "text_input"),
            Markup.button.callback("Gambar", "image_input"),
        ])
    );
});

bot.action("different", (ctx) => {
    selectedLanguage = "";
    ctx.reply(
        "Pilih bahasa yang ingin dijadikan terjemahan :",
        generateLanguageMarkup()
    );
});

function getKeyByValue(object, value) {
    return Object.keys(object).find((key) => object[key] === value);
}

bot.launch();

function downloadFile(url, filePath) {
    return new Promise((resolve, reject) => {
        const file = fs.createWriteStream(filePath);
        https
            .get(url, (response) => {
                response.pipe(file);
                file.on("finish", () => {
                    file.close(resolve);
                });
            })
            .on("error", (err) => {
                fs.unlink(filePath);
                reject(err);
            });
    });
}
